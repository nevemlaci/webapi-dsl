using WebAPI_DSL_Lib;
using WebAPI_DSL_Lib.Generator;
using WebAPI_DSL_Lib.Model;

namespace WebAPI_DSL_Main;

public class GeneratorSelector
{
    private readonly Dictionary<string, Func<ISourceGenerator>> _generators = new()
    {
        { "aspnet", () => new AspNetGenerator.AspNetGenerator() },
    };

    public ISourceGenerator? GetGenerator(string key)
    {
        return _generators[key]?.Invoke();
    }

}