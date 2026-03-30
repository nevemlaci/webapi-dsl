namespace WebAPI_DSL_Lib.Meta.Annotations;

public class TooFewArgumentsException : Exception
{

    public TooFewArgumentsException(int minimum, int provided) :
        base($"Too few arguments provided for argument! Minimum {minimum} expected, {provided} provided.")
    {}
}

public class TooManyArgumentsException : Exception
{

    public TooManyArgumentsException(int maximum, int provided) :
        base($"Too many arguments provided for argument! Maximum {maximum} expected, {provided} provided.")
    {}
}

public class ArgumentNotFoundException : Exception
{
    public ArgumentNotFoundException()
    {
    }

    public ArgumentNotFoundException(string argumentName, int idx) 
        : base($"Argument '{argumentName}' (positional argument {idx}) was not found in the parameter list!")
    {
    }
}

public class IncorrectArgumentTypeException : Exception
{
    public IncorrectArgumentTypeException()
    {
    }

    public IncorrectArgumentTypeException(string expectedType, string paramName, int paramIdx) : 
        base($"This annotation expected a {expectedType} for argument #{paramIdx} {paramName}")
    {
    }
}