using System.Collections.Immutable;
using AspNetGenerator.SourceDataClasses;
using WebAPI_DSL_Lib.Meta;
using WebAPI_DSL_Lib.Meta.Enums;
using WebAPI_DSL_Lib.Model;
using WebAPI_DSL_Lib.Plugins.Attributes;

namespace AspNetGenerator;

[Model("aspnet")]
public class AspNetModel : IAspNetModel
{

    public IAspNetModel.ModelConfig Config => new(
        DbContextNamespace: DbContextNamespace,
        EntityNamespace: EntityNamespace,
        ControllerNamespace: ControllerNamespace,
        DtoNamespace: DtoNamespace,
        MappingNamespace: MappingNamespace,
        EnumNamespace: EnumNamespace
    );
    
    private DbContextSource dbContextSource = new();
    private List<ControllerSource> controllers = [];
    private List<DtoSource> dtos = [];
    private List<EntitySource> entities = [];
    private List<EnumSource> enums = [];
    private string ControllerNamespace { get;}
    private string DtoNamespace { get;}
    private string DbContextNamespace { get;}
    private string EntityNamespace { get;}
    private string MappingNamespace { get; }
    private string EnumNamespace { get; }

    public IImmutableList<ControllerSource> Controllers => controllers.ToImmutableList();
    public IImmutableList<DtoSource> Dtos => dtos.ToImmutableList();
    public IImmutableList<EntitySource> Entities => entities.ToImmutableList();
    public IImmutableList<EnumSource> Enums => enums.ToImmutableList();
    public DbContextSource DbContext => dbContextSource;

    public AspNetModel(DomainModel model)
    {
        ControllerNamespace = model.Config["baseNamespace"] + ".Generated.Controllers";
        DtoNamespace = model.Config["baseNamespace"] + ".Generated.Dtos";
        DbContextNamespace = model.Config["baseNamespace"] + ".Generated.DbContext";
        EntityNamespace = model.Config["baseNamespace"] + ".Generated.Entities";
        MappingNamespace = model.Config["baseNamespace"] + ".Generated.Mappings";
        EnumNamespace =  model.Config["baseNamespace"] + ".Generated.Enums";
        
        foreach (var enum_ in model.UserDefinedEnums)
        {
            var enumSource = new EnumSource() { Name = enum_.Name, Values = enum_.Values };
            enums.Add(enumSource);
        }
        
        foreach (var modelEntity in model.Entities)
        {
            if(!modelEntity.GenerateDefaultCrud) continue;
            var route = modelEntity.Route ?? modelEntity.Name.ToLower();
            var entity = new EntitySource { ClassName = NameHelper.ToPascal(modelEntity.Name)};
            var dto = new DtoSource(entity.ClassName);
            var controller = ControllerSource.CreateCrud(entity.ClassName, route, dbContextSource.ClassName);

            entity.Fields.Add(new EntityFieldSource
            {
                Name = "Id",
                Type = "Guid",
                IsPrimaryKey = true
            });
            dto.Fields.Add(new DtoFieldDefinition()
            {
                Name = "Id",
                IsList = false,
                IsRequired = false,
                Type = "Guid"
            });
            dbContextSource.DbSets.Add(
                new() { Entity = entity, EntityName = entity.ClassName, ClassName = entity.ClassName + 's' }
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
            
            var filterFields = modelEntity.Fields.Where(f => f.Filter != FilterType.EFilterType.None).ToList();
            if (filterFields.Any())
            {
                var searchAction = new ActionSource
                {
                    MethodName = $"Search{entity.ClassName}s",
                    Verb = HttpVerb.Get,
                    RouteTemplate = "search",
                    ReturnType = $"ActionResult<IEnumerable<{dto.ClassName}>>"
                };

                foreach (var f in filterFields)
                {
                    var fieldType = NameHelper.GetTypeOfField(f);
                    var pascalName = NameHelper.ToPascal(f.Name);

                    if (f.Filter == FilterType.EFilterType.Search)
                    {
                        var paramName = f.Name.ToLower();
                        searchAction.Parameters.Add((fieldType + "?", paramName));
                        searchAction.Filters.Add(new FilterInfo
                        {
                            Type = "Search",
                            FieldName = pascalName,
                            ParamName = paramName
                        });
                    }
                    else if (f.Filter == FilterType.EFilterType.Range)
                    {
                        var minParam = "min" + pascalName;
                        var maxParam = "max" + pascalName;
                        searchAction.Parameters.Add((fieldType + "?", minParam));
                        searchAction.Parameters.Add((fieldType + "?", maxParam));
                        searchAction.Filters.Add(new FilterInfo
                        {
                            Type = "Range",
                            FieldName = pascalName,
                            MinParamName = minParam,
                            MaxParamName = maxParam
                        });
                    }
                }
                controller.Actions.Add(searchAction);
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
        Name = $"{pascalFieldName}Id",
        IsList = modelField.IsList,
        IsRequired = modelField.IsRequired
    });
    
    var entityField = new EntityFieldSource
    {
        Name = pascalFieldName,
        Type = fieldTypeName,
        IsRelation = true,
        IsList = modelField.IsList,
        IsUnique = modelField.IsUnique
    };

    if (!modelField.IsList)
    {
        entityField.ForeignKeyName = $"{pascalFieldName}Id";

        dbContextSource.Relationships.Add(
            new RelationshipSource
            {
                PrincipalEntity = fieldTypeName,
                DependentEntity = entity.ClassName,
                NavigationProperty = pascalFieldName,
                CollectionProperty = modelField.IsUnique ? pascalFieldName : entity.ClassName + "s",
                ForeignKeyName = pascalFieldName + "Id",
                IsOneToOne = modelField.IsUnique 
            }
        );
    }
    else if (fieldTypeName == entity.ClassName) 
    {
        var singularName = pascalFieldName.EndsWith("s") 
            ? pascalFieldName.Substring(0, pascalFieldName.Length - 1) 
            : pascalFieldName;

        dbContextSource.Relationships.Add(
            new RelationshipSource
            {
                IsSelfReferencingManyToMany = true,
                PrincipalEntity = entity.ClassName,
                NavigationProperty = pascalFieldName,
                JoinTableName = $"{entity.ClassName}{pascalFieldName}", 
                LeftForeignKey = $"{entity.ClassName}Id",               
                RightForeignKey = $"{singularName}Id"                   
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
            IsPrimaryKey = false,
            IsUnique = modelField.IsUnique
        });

        dto.Fields.Add(new DtoFieldDefinition()
        {
            Type = fieldTypeName,
            Name = pascalFieldName,
            IsRequired = modelField.IsRequired
        });
    }

}