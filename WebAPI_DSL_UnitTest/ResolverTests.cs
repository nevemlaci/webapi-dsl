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
        private Resolver _resolver;

        [SetUp]
        public void Setup()
        {
            _model = new DomainModel();
            _annotationProcessor = new AnnotationProcessor();
            _resolver = new Resolver(_model, _annotationProcessor);
        }

        [Test]
        public void ResolveField_PrimitiveType_Success()
        {
            var entity = new EntityDefinition { Name = "TestEntity" };
            var field = new FieldDefinition { Name = "TestField", RawTypeName = "int" };
            entity.Fields.Add(field);
            _model.Entities.Add(entity);

            _resolver.Resolve();

            Assert.That(field.Type, Is.EqualTo(PrimitiveTypes.IntType));
        }

        [Test]
        public void ResolveField_EnumType_Success()
        {
            var enumDef = new EnumDefinition { Name = "TestEnum", Values = ["Val1", "Val2"] };
            _model.UserDefinedEnums.Add(enumDef);

            var entity = new EntityDefinition { Name = "TestEntity" };
            var field = new FieldDefinition { Name = "TestField", RawTypeName = "TestEnum" };
            entity.Fields.Add(field);
            _model.Entities.Add(entity);

            _resolver.Resolve();

            Assert.That(field.Type, Is.EqualTo(enumDef));
        }

        [Test]
        public void ResolveField_EntityType_Success()
        {
            var referencedEntity = new EntityDefinition { Name = "RefEntity" };
            _model.Entities.Add(referencedEntity);

            var entity = new EntityDefinition { Name = "TestEntity" };
            var field = new FieldDefinition { Name = "TestField", RawTypeName = "RefEntity" };
            entity.Fields.Add(field);
            _model.Entities.Add(entity);

            _resolver.Resolve();

            Assert.That(field.Type, Is.EqualTo(referencedEntity));
        }

        [Test]
        public void ResolveField_UnknownType_ThrowsResolverError()
        {
            var entity = new EntityDefinition { Name = "TestEntity" };
            var field = new FieldDefinition { Name = "TestField", RawTypeName = "UnknownType", LineInfo = new WebAPI_DSL_Lib.Info.LineInfo(1, 1) };
            entity.Fields.Add(field);
            _model.Entities.Add(entity);

            var ex = Assert.Throws<ResolverError>(() => _resolver.Resolve());
            Assert.That(ex.Message, Does.Contain("Unknown type: UnknownType"));
        }

        [Test]
        public void ResolveExpression_EnumExpression_Success()
        {
            var enumDef = new EnumDefinition { Name = "TestEnum", Values = ["Val1"] };
            _model.UserDefinedEnums.Add(enumDef);

            var entity = new EntityDefinition { Name = "TestEntity" };
            var field = new FieldDefinition { Name = "TestField", RawTypeName = "int" };
            
            var enumExpr = new EnumExpression { RawEnumType = "TestEnum", EnumValue = "Val1" };
            field.AnnotationsRaw.Add(("TestAnnotation", new AnnotationArgumentHolder(new Dictionary<string, IExpression> { { "arg", enumExpr } })));

            entity.Fields.Add(field);
            _model.Entities.Add(entity);

            _resolver.Resolve();

            Assert.That(enumExpr.EnumType, Is.EqualTo(enumDef));
        }

        [Test]
        public void ResolveExpression_EnumExpression_UnknownEnum_LeavesEnumTypeNull()
        {
            var entity = new EntityDefinition { Name = "TestEntity" };
            var field = new FieldDefinition { Name = "TestField", RawTypeName = "int" };
            
            var enumExpr = new EnumExpression { RawEnumType = "NonExistentEnum", EnumValue = "Val1" };
            
            
            entity.Fields.Add(field);
            _model.Entities.Add(entity);

            _resolver.Resolve();

            Assert.That(enumExpr.EnumType, Is.Null);
        }

        [Test]
        public void ResolveEntity_WithAnnotation_ResolvesArguments()
        {
            var enumDef = new EnumDefinition { Name = "TestEnum", Values = ["Val1"] };
            _model.UserDefinedEnums.Add(enumDef);

            var entity = new EntityDefinition { Name = "TestEntity" };
            var enumExpr = new EnumExpression { RawEnumType = "TestEnum", EnumValue = "Val1" };
            
            entity.AnnotationsRaw.Add(("TestAnnotation", new AnnotationArgumentHolder(new Dictionary<string, IExpression> { { "arg", enumExpr } })));
            _model.Entities.Add(entity);

            _resolver.Resolve();

            Assert.That(enumExpr.EnumType, Is.EqualTo(enumDef));
        }

        [Test]
        public void ResolveField_WithMultipleAnnotations_ResolvesAllArguments()
        {
            var enumDef1 = new EnumDefinition { Name = "Enum1", Values = ["Val1"] };
            var enumDef2 = new EnumDefinition { Name = "Enum2", Values = ["Val2"] };
            _model.UserDefinedEnums.Add(enumDef1);
            _model.UserDefinedEnums.Add(enumDef2);

            var entity = new EntityDefinition { Name = "TestEntity" };
            var field = new FieldDefinition { Name = "TestField", RawTypeName = "int" };
            
            var enumExpr1 = new EnumExpression { RawEnumType = "Enum1", EnumValue = "Val1" };
            var enumExpr2 = new EnumExpression { RawEnumType = "Enum2", EnumValue = "Val2" };
            
            field.AnnotationsRaw.Add(("Anno1", new AnnotationArgumentHolder(new Dictionary<string, IExpression> { { "arg", enumExpr1 } })));
            field.AnnotationsRaw.Add(("Anno2", new AnnotationArgumentHolder(new Dictionary<string, IExpression> { { "arg", enumExpr2 } })));
            
            entity.Fields.Add(field);
            _model.Entities.Add(entity);

            _resolver.Resolve();

            Assert.That(enumExpr1.EnumType, Is.EqualTo(enumDef1));
            Assert.That(enumExpr2.EnumType, Is.EqualTo(enumDef2));
        }

        [Test]
        public void ResolveEnum_DuplicateName_ThrowsResolverError()
        {
            var enumDef1 = new EnumDefinition { Name = "DuplicateEnum", LineInfo = new WebAPI_DSL_Lib.Info.LineInfo(1, 1) };
            var enumDef2 = new EnumDefinition { Name = "DuplicateEnum", LineInfo = new WebAPI_DSL_Lib.Info.LineInfo(2, 1) };
            
            _model.UserDefinedEnums.Add(enumDef1);
            _model.UserDefinedEnums.Add(enumDef2);

            var ex = Assert.Throws<ResolverError>(() => _resolver.Resolve());
            Assert.That(ex.Message, Does.Contain("Enum name DuplicateEnum is not unique!"));
        }

        [Test]
        public void ResolveEntity_DuplicateName_ThrowsResolverError()
        {
            var entity1 = new EntityDefinition { Name = "DuplicateEntity", LineInfo = new WebAPI_DSL_Lib.Info.LineInfo(1, 1) };
            var entity2 = new EntityDefinition { Name = "DuplicateEntity", LineInfo = new WebAPI_DSL_Lib.Info.LineInfo(2, 1) };
            
            _model.Entities.Add(entity1);
            _model.Entities.Add(entity2);

            var ex = Assert.Throws<ResolverError>(() => _resolver.Resolve());
            Assert.That(ex.Message, Does.Contain("Entity name DuplicateEntity is not unique!"));
        }

        [Test]
        public void ResolveEntity_NameMatchesEnum_ThrowsResolverError()
        {
            var enumDef = new EnumDefinition { Name = "SameName" };
            _model.UserDefinedEnums.Add(enumDef);

            var entity = new EntityDefinition { Name = "SameName", LineInfo = new WebAPI_DSL_Lib.Info.LineInfo(1, 1) };
            _model.Entities.Add(entity);

            var ex = Assert.Throws<ResolverError>(() => _resolver.Resolve());
            Assert.That(ex.Message, Does.Contain("Entity name SameName is not unique!"));
        }

        [Test]
        public void ResolveEnum_NameMatchesPrimitive_ThrowsResolverError()
        {
            var enumDef = new EnumDefinition { Name = "int", LineInfo = new WebAPI_DSL_Lib.Info.LineInfo(1, 1) };
            _model.UserDefinedEnums.Add(enumDef);

            var ex = Assert.Throws<ResolverError>(() => _resolver.Resolve());
            Assert.That(ex.Message, Does.Contain("Enum name int is not unique!"));
        }

        [Test]
        public void ResolveEntity_DuplicateFieldName_ThrowsResolverError()
        {
            var entity = new EntityDefinition { Name = "TestEntity" };
            var field1 = new FieldDefinition { Name = "DuplicateField", RawTypeName = "int", LineInfo = new WebAPI_DSL_Lib.Info.LineInfo(1, 1) };
            var field2 = new FieldDefinition { Name = "DuplicateField", RawTypeName = "int", LineInfo = new WebAPI_DSL_Lib.Info.LineInfo(2, 1) };
            
            entity.Fields.Add(field1);
            entity.Fields.Add(field2);
            _model.Entities.Add(entity);

            var ex = Assert.Throws<ResolverError>(() => _resolver.Resolve());
            Assert.That(ex.Message, Does.Contain("Field name DuplicateField is not unique in entity TestEntity!"));
        }
    }
}