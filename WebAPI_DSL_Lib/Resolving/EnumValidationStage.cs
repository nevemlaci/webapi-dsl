namespace WebAPI_DSL_Lib.Resolving;

public sealed class EnumValidationStage : IResolverStage
{
    public void Execute(ResolverContext context)
    {
        foreach (var @enum in context.Model.Enums)
        {
            ResolverHelpers.ResolveEnum(context, @enum);
        }
    }
}