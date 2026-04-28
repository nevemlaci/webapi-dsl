namespace WebAPI_DSL_Lib.Resolving;

public sealed class AnnotationProcessingStage : IResolverStage
{
    public void Execute(ResolverContext context)
    {
        foreach (var entity in context.Model.Entities)
        {
            ResolverHelpers.ProcessAnnotations(context, entity);
        }
    }
}