using WebAPI_DSL_Lib.Meta.Annotations.ArgumentProcessing;
using WebAPI_DSL_Lib.Meta.Expressions;
using WebAPI_DSL_Lib.Meta.Types;
using WebAPI_DSL_Lib.Util;

namespace WebAPI_DSL_Lib.Meta.Annotations.Builtin;

public class RouteAnnotation : EntityAnnotation
{
    public override string Name => "@Route";
    private static readonly ArgumentLayout ArgumentLayout = new ArgumentLayout().Add("route", PrimitiveTypes.StringType);

    public override void Apply(object o, AnnotationArgumentHolder args)
    {
        base.Apply(o, args);
        var resolvedArgs = args.GetArgs(ArgumentLayout);
        var routeRaw = resolvedArgs["route"];
        var route = routeRaw as StringExpression;
        var routes = RouteSanitiazation.SanitizeRoute(route!.Value);
        if (o is EntityDefinition e) e.Route = routes;
    }
    
    
}