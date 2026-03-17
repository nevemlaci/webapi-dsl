using WebAPI_DSL_Lib.Meta;
using WebAPI_DSL_Lib.Meta.Enums;
using WebAPI_DSL_Lib.Meta.Types;

namespace WebAPI_DSL_Lib.Model;

public class DomainModel
{
    public Dictionary<string, string> Config { get; set; } = new();
    public List<EntityDefinition> Entities { get; } = [];
    public List<EnumDefinition> Enums { get; } = [FilterType.Definition];

    public List<PrimitiveTypeBase> PrimitiveTypes { get; } =
        [new IntType(), new BoolType(), new DoubleType(), new StringType()];
}