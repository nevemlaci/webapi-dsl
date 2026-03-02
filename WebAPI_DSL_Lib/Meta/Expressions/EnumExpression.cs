using WebAPI_DSL_Lib.Meta.Types;

namespace WebAPI_DSL_Lib.Meta.Expressions;

public class EnumExpression(string enumType, string enumValue) : MetaBase, IExpression
{
    public string RawEnumType { get; init; } = enumType;
    
    public EnumDefinition? EnumType { get; set; }
    
    public string EnumValue { get; init; } = enumValue;
}