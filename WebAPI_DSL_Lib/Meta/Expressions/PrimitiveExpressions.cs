namespace WebAPI_DSL_Lib.Meta.Expressions;

public class PrimitiveExpression<T>
{
    public T Value { get; init; }
}

public class IntExpression : PrimitiveExpression<int>;
public class DoubleExpression : PrimitiveExpression<double>;
public class BoolExpression : PrimitiveExpression<bool>;
public class StringExpression : PrimitiveExpression<string>;