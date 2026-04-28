namespace WebAPI_DSL_Lib.Resolving;

public sealed class ResolverPipeline(IEnumerable<IResolverStage> stages)
{
    private readonly List<IResolverStage> _stages = stages.ToList();

    public bool Execute(ResolverContext context)
    {
        var success = true;
        foreach (var stage in _stages)
        {
            try
            {
                stage.Execute(context);
            }
            catch (ResolverError)
            {
                success = false;
            }
        }

        return success;
    }
}