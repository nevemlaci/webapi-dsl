namespace WebAPI_DSL_Lib.Meta.Types;

public class EnumDefinition : MetaBase, IType
{
    public string Name { get; init; }
    
    public List<string> Values { get; set; } = [];
}