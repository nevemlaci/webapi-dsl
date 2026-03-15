using WebAPI_DSL_Lib.Meta.Expressions;
using WebAPI_DSL_Lib.Meta.Types;

namespace WebAPI_DSL_Lib.Meta;

public class FieldDefinition : MetaBase
{
    public string Name { get; set; }
    public string RawTypeName { get; set; }
    
    public IType? Type { get; set; }

    public bool IsList { get; set; } = false;

    public bool IsPrimaryKey { get; set; } = false;
    public bool IsRequired { get; set; } = false;
    public bool IsUnique { get; set; } = false;
    public bool IsRelation => Type is EntityDefinition;
    public IExpression? DefaultValue { get; set; }

    public List<(string name, Dictionary<string, object> args)> AnnotationsRaw { get;} = [];
    public override string ToString()
    {
        return $"{Name} : {RawTypeName}";
    }
}