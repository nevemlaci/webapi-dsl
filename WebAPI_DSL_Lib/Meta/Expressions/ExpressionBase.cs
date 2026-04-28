using WebAPI_DSL_Lib.Info;
using WebAPI_DSL_Lib.Meta.Types;

namespace WebAPI_DSL_Lib.Meta.Expressions;

public abstract class ExpressionBase : IExpression
{
    public LineInfo LineInfo { get; set; }
    public abstract IType Type { get; }
}