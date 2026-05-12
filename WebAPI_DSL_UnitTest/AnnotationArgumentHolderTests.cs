using WebAPI_DSL_Lib.Meta.Annotations;
using WebAPI_DSL_Lib.Meta.Annotations.ArgumentHolder;
using WebAPI_DSL_Lib.Meta.Expressions;
using WebAPI_DSL_Lib.Meta.Types;

namespace WebAPI_DSL_UnitTest;

public class AnnotationArgumentHolderTests
{
    /// <summary>
    /// Scenario
    /// Create an AnnotationArgumentHolder with a single int argument accessed by name
    /// Expected:
    /// GetArgs returns the argument with correct name and value
    /// </summary>
    [Test]
    public void GetArgs_SingleNamedArgument_Success()
    {
        var intExpr = new IntExpression { Value = 42 };
        var holder = new AnnotationArgumentHolder(new Dictionary<string, IExpression> { { "count", intExpr } });
        var layout = new ArgumentLayout().Add("count", PrimitiveTypes.IntType);

        var result = holder.GetArgs(layout);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result["count"], Is.EqualTo(intExpr));
    }

    /// <summary>
    /// Scenario
    /// Create an AnnotationArgumentHolder with a single int argument accessed by positional index
    /// Expected:
    /// GetArgs returns the argument with correct index
    /// </summary>
    [Test]
    public void GetArgs_PositionalArgument_Success()
    {
        var intExpr = new IntExpression { Value = 100 };
        var holder = new AnnotationArgumentHolder(new Dictionary<string, IExpression> { { "__arg1", intExpr } });
        var layout = new ArgumentLayout().Add("value", PrimitiveTypes.IntType);

        var result = holder.GetArgs(layout);

        Assert.That(result["value"], Is.EqualTo(intExpr));
    }

    /// <summary>
    /// Scenario
    /// Create an AnnotationArgumentHolder with multiple arguments of different types
    /// Expected:
    /// GetArgs returns all arguments in correct order
    /// </summary>
    [Test]
    public void GetArgs_MultipleArgumentsDifferentTypes_Success()
    {
        var intExpr = new IntExpression { Value = 42 };
        var stringExpr = new StringExpression { Value = "test" };
        var boolExpr = new BoolExpression { Value = true };

        var holder = new AnnotationArgumentHolder(new Dictionary<string, IExpression>
        {
            { "count", intExpr },
            { "name", stringExpr },
            { "enabled", boolExpr }
        });

        var layout = new ArgumentLayout()
            .Add("count", PrimitiveTypes.IntType)
            .Add("name", PrimitiveTypes.StringType)
            .Add("enabled", PrimitiveTypes.BoolType);

        var result = holder.GetArgs(layout);

        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result["count"], Is.EqualTo(intExpr));
        Assert.That(result["name"], Is.EqualTo(stringExpr));
        Assert.That(result["enabled"], Is.EqualTo(boolExpr));
    }

    /// <summary>
    /// Scenario
    /// Create an AnnotationArgumentHolder with no arguments and null layout
    /// Expected:
    /// GetArgs returns empty dictionary
    /// </summary>
    [Test]
    public void GetArgs_NullLayout_ReturnsEmptyDict()
    {
        var holder = new AnnotationArgumentHolder(new Dictionary<string, IExpression>());

        var result = holder.GetArgs(null);

        Assert.That(result.Count, Is.EqualTo(0));
    }

    /// <summary>
    /// Scenario
    /// Create an AnnotationArgumentHolder with fewer arguments than required by layout
    /// Expected:
    /// GetArgs throws TooFewArgumentsException
    /// </summary>
    [Test]
    public void GetArgs_TooFewArguments_ThrowsTooFewArgumentsException()
    {
        var holder = new AnnotationArgumentHolder(new Dictionary<string, IExpression>());
        var layout = new ArgumentLayout()
            .Add("arg1", PrimitiveTypes.IntType)
            .Add("arg2", PrimitiveTypes.StringType);

        var ex = Assert.Throws<TooFewArgumentsException>(() => holder.GetArgs(layout));
        Assert.That(ex.Message, Does.Contain("Too few arguments"));
    }

    /// <summary>
    /// Scenario
    /// Create an AnnotationArgumentHolder with more arguments than layout allows
    /// Expected:
    /// GetArgs throws TooManyArgumentsException
    /// </summary>
    [Test]
    public void GetArgs_TooManyArguments_ThrowsTooManyArgumentsException()
    {
        var intExpr1 = new IntExpression { Value = 1 };
        var intExpr2 = new IntExpression { Value = 2 };
        var intExpr3 = new IntExpression { Value = 3 };

        var holder = new AnnotationArgumentHolder(new Dictionary<string, IExpression>
        {
            { "arg1", intExpr1 },
            { "arg2", intExpr2 },
            { "arg3", intExpr3 }
        });

        var layout = new ArgumentLayout()
            .Add("arg1", PrimitiveTypes.IntType)
            .Add("arg2", PrimitiveTypes.IntType);

        var ex = Assert.Throws<TooManyArgumentsException>(() => holder.GetArgs(layout));
        Assert.That(ex.Message, Does.Contain("Too many arguments"));
    }

    /// <summary>
    /// Scenario
    /// Create an AnnotationArgumentHolder where an argument type does not match layout expectation
    /// Expected:
    /// GetArgs throws IncorrectArgumentTypeException
    /// </summary>
    [Test]
    public void GetArgs_IncorrectArgumentType_ThrowsIncorrectArgumentTypeException()
    {
        var stringExpr = new StringExpression { Value = "not an int" };
        var holder = new AnnotationArgumentHolder(new Dictionary<string, IExpression> { { "value", stringExpr } });
        var layout = new ArgumentLayout().Add("value", PrimitiveTypes.IntType);

        var ex = Assert.Throws<IncorrectArgumentTypeException>(() => holder.GetArgs(layout));
        Assert.That(ex.Message, Does.Contain("int"));
    }

    /// <summary>
    /// Scenario
    /// Create an AnnotationArgumentHolder where a required argument is missing
    /// Expected:
    /// GetArgs throws ArgumentNotFoundException
    /// </summary>
    [Test]
    public void GetArgs_RequiredArgumentNotFound_ThrowsArgumentNotFoundException()
    {
        var intExpr = new IntExpression { Value = 42 };
        var holder = new AnnotationArgumentHolder(new Dictionary<string, IExpression>
        {
            { "notTheNameWeAreLookingFor", intExpr },
        });
        var layout = new ArgumentLayout().Add("requiredArg", PrimitiveTypes.IntType);

        Assert.Throws<ArgumentNotFoundException>(() => holder.GetArgs(layout));
    }

    /// <summary>
    /// Scenario
    /// Create an AnnotationArgumentHolder with optional arguments where some are provided
    /// Expected:
    /// GetArgs returns only provided arguments
    /// </summary>
    [Test]
    public void GetArgs_OptionalArguments_OnlyProvidedReturned()
    {
        var intExpr = new IntExpression { Value = 42 };
        var holder = new AnnotationArgumentHolder(new Dictionary<string, IExpression> { { "required", intExpr } });

        var layout = new ArgumentLayout()
            .Add("required", PrimitiveTypes.IntType)
            .Add("optional", PrimitiveTypes.StringType, required: false);

        var result = holder.GetArgs(layout);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result["required"], Is.EqualTo(intExpr));
    }

    /// <summary>
    /// Scenario
    /// Create an AnnotationArgumentHolder with optional arguments where all are provided
    /// Expected:
    /// GetArgs returns all arguments
    /// </summary>
    [Test]
    public void GetArgs_AllOptionalArgumentsProvided_AllReturned()
    {
        var intExpr = new IntExpression { Value = 42 };
        var stringExpr = new StringExpression { Value = "test" };

        var holder = new AnnotationArgumentHolder(new Dictionary<string, IExpression>
        {
            { "required", intExpr },
            { "optional", stringExpr }
        });

        var layout = new ArgumentLayout()
            .Add("required", PrimitiveTypes.IntType)
            .Add("optional", PrimitiveTypes.StringType, required: false);

        var result = holder.GetArgs(layout);

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result["required"], Is.EqualTo(intExpr));
        Assert.That(result["optional"], Is.EqualTo(stringExpr));
    }

    /// <summary>
    /// Scenario
    /// Create an AnnotationArgumentHolder and access it as a Dictionary
    /// Expected:
    /// Dictionary operations work correctly
    /// </summary>
    [Test]
    public void AsDictionary_AccessViaIndexer_Success()
    {
        var intExpr = new IntExpression { Value = 42 };
        var holder = new AnnotationArgumentHolder(new Dictionary<string, IExpression> { { "myArg", intExpr } });

        Assert.That(holder["myArg"], Is.EqualTo(intExpr));
    }

    /// <summary>
    /// Scenario
    /// Create an AnnotationArgumentHolder and check Count property
    /// Expected:
    /// Count returns correct number of arguments
    /// </summary>
    [Test]
    public void Count_MultipleArguments_ReturnsCorrectCount()
    {
        var holder = new AnnotationArgumentHolder(new Dictionary<string, IExpression>
        {
            { "arg1", new IntExpression { Value = 1 } },
            { "arg2", new StringExpression { Value = "test" } },
            { "arg3", new BoolExpression { Value = true } }
        });

        Assert.That(holder.Count, Is.EqualTo(3));
    }
}