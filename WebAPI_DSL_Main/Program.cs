using Antlr4.Runtime;
using CommandLine;
using WebAPI_DSL_Lib.Model;
using WebAPI_DSL_Main.Visitor;

namespace WebAPI_DSL_Main;

internal static class Program
{
    
    static void Main(string[] args)
    {
        var filename = "test.restapi";
        CommandLine.Parser.Default.ParseArguments<Args>(args)
            .WithParsed(options =>
                {
                    if (options.InputFile != null)
                    {
                        filename = options.InputFile;
                    }
                }

            );

        var src = File.ReadAllText(filename);

        var inputStream = new AntlrInputStream(src.ToCharArray(), src.Length);

        var lexer = new RestDslLexer(inputStream);
        var tokenStream = new CommonTokenStream(lexer);

        var parser = new RestDslParser(tokenStream);

        var context = parser.program();

        var visitor = new VisitorImpl();

        var model = visitor.VisitProgram(context);

        var resolver = new Resolver(model);
        
        resolver.Resolve();

        GeneratorSelector generatorSelector = new();
        var generator = generatorSelector.GetGenerator("aspnet", model);
        
        generator?.Codegen();
    }
}