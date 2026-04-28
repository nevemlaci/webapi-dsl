using WebAPI_DSL_Lib.Generator;

namespace WebAPI_DSL_Main;

public class GeneratorSelector
{
    private readonly Dictionary<string, Func<ISourceGenerator>> _generators = new()
    {
        { "aspnet", () => new AspNetGenerator.AspNetGenerator() },
    };

    public void RegisterGenerator(string name, Func<ISourceGenerator> factory)
    {
        _generators[name] = factory;
    }
    
    public ISourceGenerator? GetGenerator(string key)
    {
        if (!_generators.TryGetValue(key, out var gen))
        {
            return null;
        }
        
        return gen.Invoke();
    }

}