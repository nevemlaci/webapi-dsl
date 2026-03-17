using WebAPI_DSL_Lib.Meta.Enums;
using WebAPI_DSL_Lib.Meta.Expressions;

namespace WebAPI_DSL_Lib.Meta.Annotations;

public class FilterAnnotation : EntityAnnotation
{
    public override string Name => "@Filter";

    public override void Apply(object o, Dictionary<string, object> args)
    {
        if (!args.TryGetValue("type", out var arg1) && !args.TryGetValue("__arg1", out arg1))
        {
            throw new ArgumentNotFoundException("type");
        }

        var e = arg1 as EnumExpression;

        if (e == null) throw new ArgumentException();

        var f = (FieldDefinition)o;
        
        switch (e.EnumValue)
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