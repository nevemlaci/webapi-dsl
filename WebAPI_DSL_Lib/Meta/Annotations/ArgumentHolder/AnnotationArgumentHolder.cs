using WebAPI_DSL_Lib.Meta.Expressions;
using WebAPI_DSL_Lib.Meta.Types;

namespace WebAPI_DSL_Lib.Meta.Annotations.ArgumentHolder;

public class AnnotationArgumentHolder : Dictionary<string, IExpression>
{
    public enum ArgumentLookupResult
    {
        Success, IncorrectType, NotFound
    }
    public AnnotationArgumentHolder(Dictionary<string, IExpression> d) : base(d)
    {
        
    }
    
    private ArgumentLookupResult TryGetArg(string name, int idx, IType type, out IExpression? value)
    {
        if (!TryGetValue(name, out var v) && !TryGetValue($"__arg{idx}", out v))
        {

            value = null;
            return ArgumentLookupResult.NotFound;
        }
        
        if (v.Type != type)
        {
            value = null;
            return ArgumentLookupResult.IncorrectType;
        }
        
        value = v;
        return ArgumentLookupResult.Success;
    }
    
    /// <summary>
    /// Get the args of an annotation based on an argument layout.
    /// If the argument layout is null, this function is basically an
    /// assertion to check if the parameter list is empty.
    /// </summary>
    /// <param name="layout"></param>
    /// <returns></returns>
    /// <exception cref="TooFewArgumentsException"></exception>
    /// <exception cref="TooManyArgumentsException"></exception>
    /// <exception cref="IncorrectArgumentTypeException"></exception>
    /// <exception cref="ArgumentNotFoundException"></exception>
    public Dictionary<string, IExpression?> GetArgs(ArgumentLayout? layout)
    {
        if (layout == null)
        {
            layout = new ArgumentLayout();
        }
        
        if (Count < layout.MinimumArgumentCount)
        {
            throw new TooFewArgumentsException(layout.MinimumArgumentCount, Count);
        }

        if (Count > layout.MaximumArgumentCount)
        {
            throw new TooManyArgumentsException(layout.MaximumArgumentCount, Count);
        }
        
        Dictionary<string, IExpression?> result = [];
        
        foreach (var arg in layout.Layout)
        {
            var argLookupResult = TryGetArg(arg.Name, arg.Position, arg.Type, out var value);
            switch (argLookupResult)
            {
                case ArgumentLookupResult.IncorrectType:
                    throw new IncorrectArgumentTypeException(arg.Type.Name, arg.Name, arg.Position);
                case ArgumentLookupResult.NotFound:
                    if(arg.Required) throw new ArgumentNotFoundException(arg.Name, arg.Position);
                    result[arg.Name] = null;
                    break;
                case ArgumentLookupResult.Success:
                    result[arg.Name] = value;
                    break;
            }
        }

        return result;
    }
}