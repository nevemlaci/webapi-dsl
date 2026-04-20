using WebAPI_DSL_Lib;
using WebAPI_DSL_Lib.Generator;

namespace WebAPI_DSL_Main;

public class GeneratorSelector
{
    private Logger logger = new("GeneratorSelector");
    
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
            logger.Error(null, $"Couldn't find generator named \"{key}\"");
            return null;
        }
        
        logger.Info($"Found generator {key}");
        return gen.Invoke();
    }

}