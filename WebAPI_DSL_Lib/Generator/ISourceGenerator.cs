using WebAPI_DSL_Lib.Model;
using WebAPI_DSL_Lib.Plugins;

namespace WebAPI_DSL_Lib.Generator;

public interface ISourceGenerator
{
    public string WorkingDirectory { get; set; }
    public void Codegen(string outputDir, DomainModel domainModel);
    public void Codegen(string outputDir, object generatorSpecificModel);
}
