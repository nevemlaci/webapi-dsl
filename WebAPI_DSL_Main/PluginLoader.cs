using System.Reflection;
using WebAPI_DSL_Lib.Model;
using WebAPI_DSL_Lib.Plugins.Attributes;
using GeneratorAttribute = WebAPI_DSL_Lib.Plugins.Attributes.GeneratorAttribute;
using ISourceGenerator = WebAPI_DSL_Lib.Generator.ISourceGenerator;

namespace WebAPI_DSL_Main;

public class PluginLoader
{
    private readonly Dictionary<string, Func<ISourceGenerator>> _generators = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Func<DomainModel, object>> _modelFactories = new(StringComparer.OrdinalIgnoreCase);
    
    public void RegisterGenerator(string name, Func<ISourceGenerator> factory)
    {
        _generators[name] = factory;
    }
    
    public void RegisterModelFactory<TModel>(string name, Func<DomainModel, TModel> factory) where TModel : notnull
    {
        _modelFactories[name] = model => factory(model);
    }
    
    public ISourceGenerator? GetGenerator(string name)
    {
        return _generators.TryGetValue(name, out var factory) ? factory() : null;
    }
    
    public object? GetModel(string name, DomainModel domainModel)
    {
        return _modelFactories.TryGetValue(name, out var factory) ? factory(domainModel) : null;
    }

    public void LoadPlugins(string pluginPath, GeneratorSelector generatorSelector, ModelSelector modelSelector)
    {
        if (!Directory.Exists(pluginPath)) return;

        var pluginFiles = Directory.GetFiles(pluginPath, "*.dll");
        foreach (var file in pluginFiles)
        {
            {
                try
                {
                    var assembly = Assembly.LoadFrom(file);

                    var models = assembly.GetTypes()
                        .Where(t => t.GetCustomAttribute(typeof(ModelAttribute)) != null
                        );

                    foreach (var model in models)
                    {
                        var key = ((ModelAttribute)model.GetCustomAttribute(typeof(ModelAttribute))!).Name;
                        RegisterModelFactory(key, (m) => Activator.CreateInstance(model, m));
                    }
                    
                    var generators = assembly.GetTypes()
                        .Where(t => t.GetCustomAttribute(typeof(Microsoft.CodeAnalysis.GeneratorAttribute)) != null
                        );
                    
                    foreach (var generator in generators)
                    {
                        var key = ((GeneratorAttribute)generator.GetCustomAttribute(typeof(GeneratorAttribute))!).Name;
                        generatorSelector.RegisterGenerator(key, () => (ISourceGenerator)Activator.CreateInstance(generator));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load plugin {file}: {ex.Message}");
                }
            }
        }
    }
}