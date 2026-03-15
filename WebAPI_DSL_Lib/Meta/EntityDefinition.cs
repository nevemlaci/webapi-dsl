using WebAPI_DSL_Lib.Meta.Annotations;
using WebAPI_DSL_Lib.Meta.Types;

namespace WebAPI_DSL_Lib.Meta;

public class EntityDefinition : MetaBase, IType
{
    public string Name { get; set; }
    public List<FieldDefinition> Fields { get; set; } = [];
    
    public List<(string name, Dictionary<string, object> args)> AnnotationsRaw { get; } = [];

    public bool GenerateDefaultCrud = true;
    
    public string? Route { get; set; }

    public override string ToString()
    {
        return Name;
    }
}