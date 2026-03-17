using WebAPI_DSL_Lib.Meta.Types;

namespace WebAPI_DSL_Lib.Meta.Enums;

public class FilterType
{
    public const string SearchType = "Search";
    public const string RangeType = "Range";

    public static readonly EnumDefinition Definition = new EnumDefinition()
        { Name = "FilterType", Values = [SearchType, RangeType] };
    
    public enum EFilterType
    {
        None, Search, Range
    }
}