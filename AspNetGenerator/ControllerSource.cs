namespace AspNetGenerator;

public class ControllerSource
{
    public string ClassName { get; set; }
    public string RoutePrefix { get; set; }
    public string EntityClassName { get; set; }
    public string DtoClassName { get; set; }
    public string DbContextName { get; set; }
    public string DbSetName { get; set; }
    public List<ActionSource> Actions { get; set; } = [];
    
    public ControllerSource(string entityName, string route, string dbContextName)
    {
        ClassName = $"{entityName}Controller";
        EntityClassName = entityName;
        DtoClassName = $"{entityName}Dto";
        DbContextName = dbContextName;
        DbSetName = $"{entityName}s";
        RoutePrefix = $"api/{route}";
    }

    public static ControllerSource CreateCrud(string entityName, string route, string dbContextName)
    {
        var source = new ControllerSource(entityName, route, dbContextName);
        
        var create = new ActionSource() {
            MethodName = $"Create{entityName}",
            Verb = HttpVerb.Post,
            RouteTemplate = "",
            ReturnType = $"ActionResult<{source.DtoClassName}>"
        };
        create.Parameters.Add((source.DtoClassName, "dto"));
        source.Actions.Add(create);
        
        source.Actions.Add(new ActionSource()
        {
            MethodName = $"Get{entityName}s",
            Verb = HttpVerb.Get,
            RouteTemplate = "",
            ReturnType = $"ActionResult<IEnumerable<{source.DtoClassName}>>"
        });
        
        var getById = new ActionSource()
        {
            MethodName = $"Get{entityName}",
            Verb = HttpVerb.Get,
            RouteTemplate = "{id}",
            ReturnType = $"ActionResult<{source.DtoClassName}>"
        };
        getById.Parameters.Add(("Guid", "id"));
        source.Actions.Add(getById);
        
        var update = new ActionSource() {
            MethodName = $"Update{entityName}",
            Verb = HttpVerb.Put,
            RouteTemplate = "{id}",
            ReturnType = "IActionResult"
        };
        update.Parameters.Add(("Guid", "id"));
        update.Parameters.Add(("Update" + source.DtoClassName, "dto"));
        source.Actions.Add(update);
        
        var delete = new ActionSource() {
            MethodName = $"Delete{entityName}",
            Verb = HttpVerb.Delete,
            RouteTemplate = "{id}",
            ReturnType = "IActionResult"
        };
        delete.Parameters.Add(("Guid", "id"));
        source.Actions.Add(delete);

        return source;
    }
}