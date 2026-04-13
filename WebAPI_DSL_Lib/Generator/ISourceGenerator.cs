using WebAPI_DSL_Lib.Model;

namespace WebAPI_DSL_Lib.Generator;

public interface ISourceGenerator
{
    public void Codegen(string outputDir, DomainModel domainModel);
}

public interface ISourceGenerator<in TModel> : ISourceGenerator
{
    void Codegen(string outputDir, TModel generatorSpecificModel);
}