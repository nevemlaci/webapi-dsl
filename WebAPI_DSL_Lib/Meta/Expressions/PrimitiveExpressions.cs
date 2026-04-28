using WebAPI_DSL_Lib.Meta.Types;

namespace WebAPI_DSL_Lib.Meta.Expressions;

public abstract class PrimitiveExpression<T>(IType type) : ExpressionBase
{
    public T Value { get; init; }
    public override IType Type { get; } = type;

    public override string ToString()
    {
        return Value?.ToString() ?? "Unknown value";
    }
}

public class IntExpression() : PrimitiveExpression<int>(PrimitiveTypes.IntType);
public class DoubleExpression() : PrimitiveExpression<double>(PrimitiveTypes.DoubleType);
public class BoolExpression() : PrimitiveExpression<bool>(PrimitiveTypes.BoolType);

public class StringExpression() : PrimitiveExpression<string>(PrimitiveTypes.StringType)
{
    public override string ToString()
    {
        return $"\"{base.ToString()}\"";
    }
}