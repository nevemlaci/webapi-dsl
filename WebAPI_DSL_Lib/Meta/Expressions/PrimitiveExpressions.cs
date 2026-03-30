using WebAPI_DSL_Lib.Meta.Types;

namespace WebAPI_DSL_Lib.Meta.Expressions;

public abstract class PrimitiveExpression<T>(IType type) : IExpression
{
    public T Value { get; init; }
    public IType Type { get; } = type;
}

public class IntExpression() : PrimitiveExpression<int>(PrimitiveTypes.IntType);
public class DoubleExpression() : PrimitiveExpression<double>(PrimitiveTypes.DoubleType);
public class BoolExpression() : PrimitiveExpression<bool>(PrimitiveTypes.BoolType);
public class StringExpression() : PrimitiveExpression<string>(PrimitiveTypes.StringType);