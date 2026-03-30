using WebAPI_DSL_Lib.Meta;
using WebAPI_DSL_Lib.Meta.Enums;
using WebAPI_DSL_Lib.Meta.Types;

namespace WebAPI_DSL_Lib.Model;

public class DomainModel
{
    public DomainModel()
    {
        cachedEnums = BuiltinEnums.Concat(UserDefinedEnums);
    }
    public Dictionary<string, string> Config { get; set; } = new();
    public List<EntityDefinition> Entities { get; } = [];

    public List<EnumDefinition> BuiltinEnums { get; } = [FilterType.Definition];
    public List<EnumDefinition> UserDefinedEnums { get; } = [];
    public IEnumerable<EnumDefinition> Enums => cachedEnums;
    private IEnumerable<EnumDefinition> cachedEnums;
    public List<IType> Primitives { get; } =
        [PrimitiveTypes.IntType, PrimitiveTypes.BoolType, PrimitiveTypes.DoubleType, PrimitiveTypes.StringType];
}