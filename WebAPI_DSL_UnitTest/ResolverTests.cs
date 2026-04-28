using WebAPI_DSL_Lib;
using WebAPI_DSL_Lib.Info;
using WebAPI_DSL_Lib.Meta;
using WebAPI_DSL_Lib.Meta.Annotations;
using WebAPI_DSL_Lib.Meta.Annotations.ArgumentHolder;
using WebAPI_DSL_Lib.Meta.Enums;
using WebAPI_DSL_Lib.Meta.Expressions;
using WebAPI_DSL_Lib.Meta.Types;
using WebAPI_DSL_Lib.Model;
using WebAPI_DSL_Lib.Resolving;

namespace WebAPI_DSL_UnitTest;

public class ResolverTests
{
    public class Tests
    {
        private DomainModel _model;
        private AnnotationProcessor _annotationProcessor;
        private Logger _logger;
        private ResolverContext _context;

        [SetUp]
        public void Setup()
        {
            _model = new DomainModel();
            _annotationProcessor = new AnnotationProcessor();
            _logger = new Logger("ResolverTests");
            _logger.Suppress = true;
            _context = new ResolverContext(_model, _annotationProcessor, _logger);
        }

        private static EntityDefinition CreateEntity(string name, params FieldDefinition[] fields)
        {
            var entity = new EntityDefinition { Name = name };
            foreach (var field in fields)
            {
                entity.Fields.Add(field);
            }

            return entity;
        }

        private static FieldDefinition CreateField(string name, string rawTypeName, LineInfo? lineInfo = null)
            => new() { Name = name, RawTypeName = rawTypeName, LineInfo = lineInfo ?? new LineInfo(1, 1) };

        [Test]
        public void EnumValidationStage_PrimitiveTypeAllowed()
        {
            var stage = new EnumValidationStage();
            var enumDef = new EnumDefinition { Name = "int", Values = ["Val1", "Val2"] };
            _model.UserDefinedEnums.Add(enumDef);

            Assert.Throws<ResolverError>(() => stage.Execute(_context));
        }

        [Test]
        public void EnumValidationStage_DuplicateName_ThrowsResolverError()
        {
            var stage = new EnumValidationStage();
            var enumDef1 = new EnumDefinition { Name = "DuplicateEnum", LineInfo = new LineInfo(1, 1) };
            var enumDef2 = new EnumDefinition { Name = "DuplicateEnum", LineInfo = new LineInfo(2, 1) };

            _model.UserDefinedEnums.Add(enumDef1);
            _model.UserDefinedEnums.Add(enumDef2);

            Assert.Throws<ResolverError>(() => stage.Execute(_context));
        }

        [Test]
        public void EnumValidationStage_NameMatchesPrimitive_ThrowsResolverError()
        {
            var stage = new EnumValidationStage();
            var enumDef = new EnumDefinition { Name = "int", LineInfo = new LineInfo(1, 1) };
            _model.UserDefinedEnums.Add(enumDef);

            Assert.Throws<ResolverError>(() => stage.Execute(_context));
        }

        [Test]
        public void EntityValidationStage_PrimitiveFieldTypeResolved()
        {
            var stage = new EntityValidationStage();
            var entity = CreateEntity("TestEntity", CreateField("TestField", "int"));
            _model.Entities.Add(entity);

            stage.Execute(_context);

            Assert.That(entity.Fields[0].Type, Is.EqualTo(PrimitiveTypes.IntType));
        }

        [Test]
        public void EntityValidationStage_EnumFieldTypeResolved()
        {
            var stage = new EntityValidationStage();
            var enumDef = new EnumDefinition { Name = "TestEnum", Values = ["Val1", "Val2"] };
            _model.UserDefinedEnums.Add(enumDef);

            var entity = CreateEntity("TestEntity", CreateField("TestField", "TestEnum"));
            _model.Entities.Add(entity);

            stage.Execute(_context);

            Assert.That(entity.Fields[0].Type, Is.EqualTo(enumDef));
        }

        [Test]
        public void EntityValidationStage_EntityFieldTypeResolved()
        {
            var stage = new EntityValidationStage();
            var referencedEntity = new EntityDefinition { Name = "RefEntity" };
            _model.Entities.Add(referencedEntity);

            var entity = CreateEntity("TestEntity", CreateField("TestField", "RefEntity"));
            _model.Entities.Add(entity);

            stage.Execute(_context);

            Assert.That(entity.Fields[0].Type, Is.EqualTo(referencedEntity));
        }

        [Test]
        public void EntityValidationStage_UnknownType_ThrowsResolverError()
        {
            var stage = new EntityValidationStage();
            var entity = CreateEntity("TestEntity", CreateField("TestField", "UnknownType", new LineInfo(1, 1)));
            _model.Entities.Add(entity);

            var ex = Assert.Throws<ResolverError>(() => stage.Execute(_context));
            Assert.That(ex.Message, Does.Contain("Unknown type: UnknownType"));
        }

        [Test]
        public void EntityValidationStage_DuplicateEntityName_ThrowsResolverError()
        {
            var stage = new EntityValidationStage();
            var entity1 = new EntityDefinition { Name = "DuplicateEntity", LineInfo = new LineInfo(1, 1) };
            var entity2 = new EntityDefinition { Name = "DuplicateEntity", LineInfo = new LineInfo(2, 1) };

            _model.Entities.Add(entity1);
            _model.Entities.Add(entity2);

            var ex = Assert.Throws<ResolverError>(() => stage.Execute(_context));
            Assert.That(ex.Message, Does.Contain("Entity name DuplicateEntity is not unique!"));
        }

        [Test]
        public void EntityValidationStage_NameMatchesEnum_ThrowsResolverError()
        {
            var stage = new EntityValidationStage();
            var enumDef = new EnumDefinition { Name = "SameName" };
            _model.UserDefinedEnums.Add(enumDef);

            var entity = new EntityDefinition { Name = "SameName", LineInfo = new LineInfo(1, 1) };
            _model.Entities.Add(entity);

            var ex = Assert.Throws<ResolverError>(() => stage.Execute(_context));
            Assert.That(ex.Message, Does.Contain("Entity name SameName is not unique!"));
        }

        [Test]
        public void EntityValidationStage_DuplicateFieldName_ThrowsResolverError()
        {
            var stage = new EntityValidationStage();
            var entity = CreateEntity(
                "TestEntity",
                CreateField("DuplicateField", "int", new LineInfo(1, 1)),
                CreateField("DuplicateField", "int", new LineInfo(2, 1)));
            _model.Entities.Add(entity);

            var ex = Assert.Throws<ResolverError>(() => stage.Execute(_context));
            Assert.That(ex.Message, Does.Contain("Field name DuplicateField is not unique in entity TestEntity!"));
        }

        [Test]
        public void ExpressionResolvingStage_ResolvesEnumExpressionArgumentsOnFieldAnnotations()
        {
            var stage = new EntityValidationStage();
            var enumDef = new EnumDefinition { Name = "Enum1", Values = ["Val1"] };
            _model.UserDefinedEnums.Add(enumDef);

            var enumExpr = new EnumExpression { RawEnumType = "Enum1", EnumValue = "Val1" };
            var field = CreateField("TestField", "int");
            field.AnnotationsRaw.Add(("Anno1", new AnnotationArgumentHolder(new Dictionary<string, IExpression> { { "arg", enumExpr } })));

            var entity = CreateEntity("TestEntity", field);
            _model.Entities.Add(entity);

            stage.Execute(_context);

            Assert.That(enumExpr.EnumType, Is.EqualTo(enumDef));
        }

        [Test]
        public void ExpressionResolvingStage_ResolvesEnumExpressionArgumentsOnEntityAnnotations()
        {
            var stage = new EntityValidationStage();
            var enumDef = new EnumDefinition { Name = "TestEnum", Values = ["Val1"] };
            _model.UserDefinedEnums.Add(enumDef);

            var enumExpr = new EnumExpression { RawEnumType = "TestEnum", EnumValue = "Val1" };
            var entity = new EntityDefinition { Name = "TestEntity" };
            entity.AnnotationsRaw.Add(("TestAnnotation", new AnnotationArgumentHolder(new Dictionary<string, IExpression> { { "arg", enumExpr } })));
            _model.Entities.Add(entity);

            stage.Execute(_context);

            Assert.That(enumExpr.EnumType, Is.EqualTo(enumDef));
        }

        [Test]
        public void ExpressionResolvingStage_ResolvesAllAnnotationArguments()
        {
            var stage = new EntityValidationStage();
            var enumDef1 = new EnumDefinition { Name = "Enum1", Values = ["Val1"] };
            var enumDef2 = new EnumDefinition { Name = "Enum2", Values = ["Val2"] };
            _model.UserDefinedEnums.Add(enumDef1);
            _model.UserDefinedEnums.Add(enumDef2);

            var enumExpr1 = new EnumExpression { RawEnumType = "Enum1", EnumValue = "Val1" };
            var enumExpr2 = new EnumExpression { RawEnumType = "Enum2", EnumValue = "Val2" };

            var field = CreateField("TestField", "int");
            field.AnnotationsRaw.Add(("Anno1", new AnnotationArgumentHolder(new Dictionary<string, IExpression> { { "arg", enumExpr1 } })));
            field.AnnotationsRaw.Add(("Anno2", new AnnotationArgumentHolder(new Dictionary<string, IExpression> { { "arg", enumExpr2 } })));

            var entity = CreateEntity("TestEntity", field);
            _model.Entities.Add(entity);

            stage.Execute(_context);

            Assert.That(enumExpr1.EnumType, Is.EqualTo(enumDef1));
            Assert.That(enumExpr2.EnumType, Is.EqualTo(enumDef2));
        }

        [Test]
        public void ExpressionResolvingStage_UnknownEnumInExpression_LeavesEnumTypeNull()
        {
            var stage = new AnnotationProcessingStage();
            var enumExpr = new EnumExpression { RawEnumType = "NonExistentEnum", EnumValue = "Val1" };
            var field = CreateField("TestField", "int");
            field.AnnotationsRaw.Add(("TestAnnotation", new AnnotationArgumentHolder(new Dictionary<string, IExpression> { { "arg", enumExpr } })));

            var entity = CreateEntity("TestEntity", field);
            _model.Entities.Add(entity);

            stage.Execute(_context);

            Assert.That(enumExpr.EnumType, Is.Null);
        }
    }
}