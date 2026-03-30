using System.Diagnostics;
using System.Text.RegularExpressions;
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

        var matches = Regex.Matches(input, @"[A-Z]*[a-z0-9]+|[A-Z0-9]+");

        var words = matches
            .Select(match => char.ToUpper(match.Value[0]) + match.Value.Substring(1).ToLower());

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