using WebAPI_DSL_Lib.Model;

namespace WebAPI_DSL_Lib;

public interface ISourceGenerator
{
    public void Codegen(string outputDir);
}