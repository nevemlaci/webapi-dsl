namespace WebAPI_DSL_Lib.Meta.Annotations;

public class IncorrectArgumentCountException : Exception
{
    public IncorrectArgumentCountException()
    {
    }

    public IncorrectArgumentCountException(int expectedParamCount, int givenParamCount) : 
        base($"This annotation takes {expectedParamCount} arguments but was gives {givenParamCount}.")
    {
        
    }
}

public class ArgumentNotFoundException : Exception
{
    public ArgumentNotFoundException()
    {
    }

    public ArgumentNotFoundException(string argumentName) 
        : base($"Argument '{argumentName}' was not found in the parameter list!")
    {
    }
}

public class IncorrectArgumentTypeException : Exception
{
    public IncorrectArgumentTypeException()
    {
    }

    public IncorrectArgumentTypeException(string expectedType, int paramIdx) : 
        base($"This annotation expected a {expectedType} for argument #{paramIdx}")
    {
    }
}