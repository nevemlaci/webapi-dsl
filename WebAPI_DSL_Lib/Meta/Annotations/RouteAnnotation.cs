using WebAPI_DSL_Lib.Util;

namespace WebAPI_DSL_Lib.Meta.Annotations;

public class RouteAnnotation : EntityAnnotation
{
    public override string Name => "@Route";

    public override void Apply(object o, Dictionary<string, object> args)
    {
        base.Apply(o, args);
        if (args.Count != 1)
        {
            throw new IncorrectArgumentCountException(1, args.Count);
        }

        if (!args.TryGetValue("route", out var argValue) && !args.TryGetValue("__arg1", out argValue))
        {
            throw new ArgumentNotFoundException("route");
        }

        if (argValue is not string route)
        {
            throw new IncorrectArgumentTypeException("string", 1);
        }

        route = RouteSanitiazation.SanitizeRoute(route);
        if (o is EntityDefinition e) e.Route = route;
    }
    
    
}