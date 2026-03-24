using WebAPI_DSL_Lib.Meta.Annotations.ArgumentHolder;
using WebAPI_DSL_Lib.Meta.Enums;
using WebAPI_DSL_Lib.Meta.Expressions;

namespace WebAPI_DSL_Lib.Meta.Annotations.Builtin;

public class FilterAnnotation : EntityAnnotation
{
    public override string Name => "@Filter";
    
    private static readonly ArgumentLayout ArgumentLayout = new ArgumentLayout().Add("type", FilterType.Definition);

    public override void Apply(object o, AnnotationArgumentHolder args)
    {
        var resolvedArgs = args.GetArgs(ArgumentLayout);
        var typeArgD = ArgumentLayout.Get("type");
        var type = resolvedArgs[typeArgD.Name];

        var e = type as EnumExpression;

        var f = (FieldDefinition)o;
        
        switch (e!.EnumValue)
        {
            case FilterType.SearchType:
                f.Filter = FilterType.EFilterType.Search;
                break;
            
            case "Range":
                f.Filter = FilterType.EFilterType.Range;
                break;
        }
    }
}