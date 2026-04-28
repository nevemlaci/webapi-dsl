namespace WebAPI_DSL_Lib.Resolving;

/// <summary>
/// In this stage, enum names and values are validated.
/// </summary>
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