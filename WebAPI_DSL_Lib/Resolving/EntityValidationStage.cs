namespace WebAPI_DSL_Lib.Resolving;

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