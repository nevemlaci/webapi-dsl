namespace WebAPI_DSL_Lib.Meta.Types;

public abstract class PrimitiveTypeBase : MetaBase, IType
{
    public abstract string Name { get; }
}

