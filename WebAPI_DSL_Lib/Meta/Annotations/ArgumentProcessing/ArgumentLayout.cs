using System.Collections.Immutable;
using WebAPI_DSL_Lib.Meta.Types;

namespace WebAPI_DSL_Lib.Meta.Annotations.ArgumentProcessing;

public class ArgumentLayout
{
    public record ArgumentDescription(int Position, string Name, IType Type, bool Required = true);

    private List<ArgumentDescription> layout = [];
    private Dictionary<string, ArgumentDescription> nameHelper = [];

    public int MinimumArgumentCount => layout.Count(a => a.Required);
    public int MaximumArgumentCount => layout.Count;

    public ImmutableList<ArgumentDescription> Layout => layout.ToImmutableList();

    public ArgumentLayout Add(string name, IType type, bool required = true)
    {
        return Add(new ArgumentDescription(layout.Count+1, name, type, required));
    }

    public ArgumentLayout Add(ArgumentDescription ad)
    {
        layout.Add(ad);
        nameHelper[ad.Name] = ad;
        return this;
    }

    public ArgumentDescription Get(string name)
    {
        return nameHelper[name];
    }
}