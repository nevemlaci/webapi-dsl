using System.Reflection;
using WebAPI_DSL_Lib;
using WebAPI_DSL_Lib.Model;
using WebAPI_DSL_Lib.Plugins.Attributes;
using GeneratorAttribute = WebAPI_DSL_Lib.Plugins.Attributes.GeneratorAttribute;
using ISourceGenerator = WebAPI_DSL_Lib.Generator.ISourceGenerator;

namespace WebAPI_DSL_Main;

public class PluginLoader
{
    private Logger logger = new Logger(nameof(PluginLoader));
        
    private readonly Dictionary<string, Func<ISourceGenerator>> _generators = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Func<DomainModel, object>> _modelFactories = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _workingDirectories = new();
    
    private void RegisterGenerator(string name, Func<ISourceGenerator> factory)
    {
        logger.Info($"Registered generator: {name}");
        _generators[name] = factory;
    }
    
    private void RegisterModelFactory<TModel>(string name, Func<DomainModel, TModel> factory) where TModel : notnull
    {
        logger.Info($"Registered model: {name}");
        _modelFactories[name] = model => factory(model);
    }
    
    private void LoadPluginsFromAssembly(Assembly assembly, string workingDirectory)
    {
        logger.Info($"Loading plugins from assembly: {assembly.FullName}");
        var models = assembly.GetTypes()
            .Where(t => t.GetCustomAttribute(typeof(ModelAttribute)) != null
            );

        foreach (var model in models)
        {
            var key = ((ModelAttribute)model.GetCustomAttribute(typeof(ModelAttribute))!).Name;
            RegisterModelFactory(key, (m) => Activator.CreateInstance(model, m));
        }
        
        var generators = assembly.GetTypes()
            .Where(t => t.GetCustomAttribute(typeof(GeneratorAttribute)) != null
            );
        
        foreach (var generator in generators)
        {
            var key = ((GeneratorAttribute)generator.GetCustomAttribute(typeof(GeneratorAttribute))!).Name;
            RegisterGenerator(key, () =>
            {
                var gen = (ISourceGenerator)Activator.CreateInstance(generator);
                gen.WorkingDirectory = workingDirectory;
                return gen;
            });
        }
    }

    public void LoadPlugins(string pluginPath)
    {
        if (!Directory.Exists(pluginPath)) return;
        var pluginDirectories = Directory.GetDirectories(pluginPath);
        foreach (var dir in pluginDirectories)
        {
            var pluginFiles = Directory.GetFiles(dir, "*.dll");
            foreach (var file in pluginFiles)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(file);
                    LoadPluginsFromAssembly(assembly, dir);
                }
                catch (Exception ex)
                {
                    logger.Error($"Failed to load plugin {file}: {ex.Message}");
                }
            }
        }
    }

    public void FillGeneratorSelector(GeneratorSelector genSelector)
    {
        foreach(var (name, factory) in _generators)
        {
            genSelector.RegisterGenerator(name, factory);
        }
    }

    public void FillModelSelector(ModelSelector modelSelector)
    {
        foreach (var (name, factory) in _modelFactories)
        {
            modelSelector.RegisterModel(name, factory);
        }
    }
}