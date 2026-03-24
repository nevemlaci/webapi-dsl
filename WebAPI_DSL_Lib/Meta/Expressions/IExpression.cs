using WebAPI_DSL_Lib.Meta.Types;

namespace WebAPI_DSL_Lib.Meta.Expressions;

public interface IExpression
{
    public IType Type { get; }
}