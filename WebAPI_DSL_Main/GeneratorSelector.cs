using WebAPI_DSL_Lib;
using WebAPI_DSL_Lib.Model;

namespace WebAPI_DSL_Main;

public class GeneratorSelector
{
    private readonly Dictionary<string, Func<DomainModel, ISourceGenerator>> _generators = new()
    {
        { "aspnet", m => new AspNetGenerator.AspNetGenerator(m) },
    };

    public ISourceGenerator? GetGenerator(string key, DomainModel m)
    {
        return _generators[key]?.Invoke(m);
    }

}