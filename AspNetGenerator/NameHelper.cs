using System.Diagnostics;
using WebAPI_DSL_Lib.Meta;
using WebAPI_DSL_Lib.Meta.Types;

namespace AspNetGenerator;

public static class NameHelper
{
    private static readonly Dictionary<string, string> _primitives = new Dictionary<string, string>
    {
        { "int", "int" },
        { "double", "double" },
        {"bool", "bool"},
        {"string", "string"}
    };
    
    public static string ToPascal(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;
        
        var words = input.Split(new[] {'_', '-' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower());

        return string.Concat(words);
    }
    
    public static string GetTypeOfField(FieldDefinition field)
    {
        Debug.Assert(field.Type != null);
        Debug.Assert(field.Type.Name != null);
        if (_primitives.TryGetValue(field.Type.Name, out var value))
        {
            return value;
        }
        var pascal = ToPascal(field.Type.Name);
        return pascal;
    }
}