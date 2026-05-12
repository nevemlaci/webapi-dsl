using WebAPI_DSL_Lib.Generator;

namespace WebAPI_DSL_Main;

/// <summary>
/// Resolves generator keys to <see cref="ISourceGenerator"/> instances.
/// </summary>
public class GeneratorSelector
{
    /// <summary>
    /// Known generator factories.
    /// </summary>
    private readonly Dictionary<string, Func<ISourceGenerator>> _generators = new();

    /// <summary>
    /// Registers or replaces a generator factory under the specified name.
    /// </summary>
    /// <param name="name">The generator key to associate with the factory.</param>
    /// <param name="factory">The factory used to create the generator instance.</param>
    public void RegisterGenerator(string name, Func<ISourceGenerator> factory)
    {
        _generators[name] = factory;
    }
    
    /// <summary>
    /// Gets a generator for the specified key.
    /// </summary>
    /// <param name="key">The generator key to look up.</param>
    /// <returns>The created generator, or <c>null</c> if the key is not registered.</returns>
    public ISourceGenerator? GetGenerator(string key)
    {
        if (!_generators.TryGetValue(key, out var gen))
        {
            return null;
        }
        
        return gen.Invoke();
    }

}