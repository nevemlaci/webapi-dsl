using System.Collections.Immutable;
using WebAPI_DSL_Lib.Meta;
using WebAPI_DSL_Lib.Model;

namespace AspNetGenerator;

public class AspNetModel
{
    private DbContextSource dbContextSource = new();
    private List<ControllerSource> controllers = [];
    private List<DtoSource> dtos = [];
    private List<EntitySource> entities = [];
    public string ControllerNamespace { get; private set; }
    public string DtoNamespace { get; private set; }
    public string DbContextNamespace { get; private set; }
    public string EntityNamespace { get; private set; }

    public IImmutableList<ControllerSource> Controllers => controllers.ToImmutableList();
    public IImmutableList<DtoSource> Dtos => dtos.ToImmutableList();
    public IImmutableList<EntitySource> Entities => entities.ToImmutableList();
    public DbContextSource DbContext => dbContextSource;

    public AspNetModel(DomainModel model)
    {
        ControllerNamespace = model.Config["baseNamespace"] + "Controllers";
        DtoNamespace = model.Config["baseNamespace"] + "Dtos";
        DbContextNamespace = model.Config["baseNamespace"] + "DbContext";
        EntityNamespace = model.Config["baseNamespace"] + "Entities";

        foreach (var modelEntity in model.Entities)
        {
            var entity = new EntitySource() { ClassName = NameHelper.ToPascal(modelEntity.Name)};
            var controller = ControllerSource.CreateCrud(entity.ClassName, dbContextSource.ClassName);
            var dto = new DtoSource(entity.ClassName);

            entity.Fields.Add(new EntityFieldSource
            {
                Name = "Id",
                Type = "Guid",
                IsPrimaryKey = true
            });
            dbContextSource.DbSets.Add(
                new() { EntityName = entity.ClassName, ClassName = entity.ClassName + 's' }
            );

            foreach (var modelField in modelEntity.Fields)
            {
                if (modelField.IsRelation)
                {
                    ProcessRelationField(modelField, entity, dto);
                }
                else
                {
                    ProcessPrimitiveField(modelField, entity, dto);
                }
            }

            controllers.Add(controller);
            entities.Add(entity);
            dtos.Add(dto);
        }
    }

    private void ProcessRelationField(FieldDefinition modelField, EntitySource entity, DtoSource dto)
    {
        var pascalFieldName = NameHelper.ToPascal(modelField.Name);
        var fieldTypeName = NameHelper.GetTypeOfField(modelField);

        dto.Fields.Add(new DtoFieldDefinition
        {
            Type = "Guid",
            Name = $"{pascalFieldName}Id"
        });

        var entityField = new EntityFieldSource
        {
            Name = pascalFieldName,
            Type = fieldTypeName,
            IsRelation = true,
            IsList = modelField.IsList
        };

        if (!modelField.IsList)
        {
            entityField.ForeignKeyName = $"{pascalFieldName}Id";

            dbContextSource.Relationships.Add(
                new()
                {
                    PrincipalEntity = fieldTypeName,
                    DependentEntity = entity.ClassName,
                    NavigationProperty = pascalFieldName,
                    CollectionProperty = entity.ClassName + 's',
                    ForeignKeyName = pascalFieldName + "Id"
                }
            );
        }

        entity.Fields.Add(entityField);
    }

    private void ProcessPrimitiveField(FieldDefinition modelField, EntitySource entity, DtoSource dto)
    {
        var pascalFieldName = NameHelper.ToPascal(modelField.Name);
        var fieldTypeName = NameHelper.GetTypeOfField(modelField);

        entity.Fields.Add(new EntityFieldSource
        {
            Name = pascalFieldName,
            Type = fieldTypeName,
            IsRelation = false,
            IsPrimaryKey = false
        });

        dto.Fields.Add(new DtoFieldDefinition()
        {
            Type = fieldTypeName,
            Name = pascalFieldName
        });
    }

}