namespace WebAPI_DSL_Lib.Resolving;

public class ResolverError : Exception
{
    public ResolverError(string message) : base(message) {}
}