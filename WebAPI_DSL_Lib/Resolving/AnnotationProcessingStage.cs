namespace WebAPI_DSL_Lib.Resolving;

/// <summary>
/// In this stage, annotations are applied to their targets.
/// </summary>
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