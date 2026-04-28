namespace WebAPI_DSL_Lib.Resolving;

public interface IResolverStage
{
    void Execute(ResolverContext context);
}