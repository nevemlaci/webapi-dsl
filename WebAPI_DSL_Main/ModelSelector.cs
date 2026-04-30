using AspNetGenerator;
using WebAPI_DSL_Lib;
using WebAPI_DSL_Lib.Model;

namespace WebAPI_DSL_Main;

public class ModelSelector
{
    private Logger logger = new Logger("ModelSelector");
    
    private readonly Dictionary<string, Func<DomainModel, object>> _models = new()
    {
        { "aspnet", (m) => new AspNetModel(m) },
    };

    public void RegisterModel(string name, Func<DomainModel, object> factory)
    {
        logger.Trace($"Registered new model: {name}");
        _models[name] = factory;
    }
    
    public Func<DomainModel, object>? GetModel(string key)
    {
        if (!_models.TryGetValue(key, out var result))
        {
            logger.Warn($"Couldn't find model: {key}");
            return null;
        }
        
        logger.Info($"Found model: {key}");
        return result;
    }

}