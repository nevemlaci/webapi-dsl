namespace WebAPI_DSL_Lib.Resolving;

/// <summary>
/// In this stage, entities and their fields are validated, field type references and annotation arguments are resolved.
/// </summary>
public sealed class EntityValidationStage : IResolverStage
{
    public void Execute(ResolverContext context)
    {
        foreach (var entity in context.Model.Entities)
        {
            ResolverHelpers.ResolveEntity(context, entity);
        }
    }
}