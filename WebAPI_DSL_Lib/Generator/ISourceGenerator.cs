using WebAPI_DSL_Lib.Model;

namespace WebAPI_DSL_Lib.Generator;

public interface ISourceGenerator
{
    public void Codegen(string outputDir, DomainModel domainModel);
    public void Codegen(string outputDir, object generatorSpecificModel);
}

public abstract class SourceGenerator<TModel> : ISourceGenerator where TModel : class
{
    public abstract void Codegen(string outputDir, TModel generatorSpecificModel);

    public abstract void Codegen(string outputDir, DomainModel domainModel);

    public void Codegen(string outputDir, object generatorSpecificModel )
    {
        var model = generatorSpecificModel as TModel;
        if (model is not null)
        {
            Codegen(outputDir, model);
        }
    }
}