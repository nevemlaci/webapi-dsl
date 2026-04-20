using WebAPI_DSL_Lib.Meta.Types;

namespace WebAPI_DSL_Lib.Meta.Expressions;

public class EnumExpression : MetaBase, IExpression
{
    public string RawEnumType { get; set; }
    
    public EnumDefinition? EnumType { get; set; }
    
    public string EnumValue { get; set; }
    public IType Type => EnumType;

    public override string ToString()
    {
        return $"{EnumType.Name}::{EnumValue}";
    }
}