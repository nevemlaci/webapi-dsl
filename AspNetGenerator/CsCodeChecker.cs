using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AspNetGenerator;

public static class CsCodeChecker
{
    public static void AssertCodeCompiles(string filename, string src)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(src);
        var diagnostics = syntaxTree.GetDiagnostics();

        var errors = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();

        if (errors.Count!=0)
        {
            Console.WriteLine($"File {filename} contains errors!");
            
            foreach (var e in errors)
            {
                var lineSpan = e.Location.GetMappedLineSpan();
            
                int line = lineSpan.StartLinePosition.Line + 1;
                int column = lineSpan.StartLinePosition.Character + 1;

                Console.WriteLine($" ({line},{column}): {e.Id} : {e.GetMessage()}");
            }
        }
    }
}