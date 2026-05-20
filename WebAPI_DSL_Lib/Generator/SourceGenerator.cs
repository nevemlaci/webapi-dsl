using WebAPI_DSL_Lib.Model;

namespace WebAPI_DSL_Lib.Generator;

public abstract class SourceGenerator : ISourceGenerator
{
    public string WorkingDirectory { get; set; } = AppContext.BaseDirectory;
    
    public abstract void Codegen(string outputDir, DomainModel domainModel);

    public abstract void Codegen(string outputDir, object generatorSpecificModel);
}

public abstract class SourceGenerator<TModel> : SourceGenerator where TModel : class
{
    public abstract void Codegen(string outputDir, TModel generatorSpecificModel);
    
    public override void Codegen(string outputDir, object generatorSpecificModel )
    {
        var model = generatorSpecificModel as TModel;
        if (model is not null)
        {
            Codegen(outputDir, model);
        }
    }
}