using Antlr4.Runtime;
using WebAPI_DSL_Lib;
using WebAPI_DSL_Lib.Info;
using WebAPI_DSL_Lib.Model;
using WebAPI_DSL_Main.SyntaxErrorHandlers;
using WebAPI_DSL_Main.Visitor;

namespace WebAPI_DSL_Main;

/// <summary>
/// Structured parse diagnostic.
/// </summary>
public sealed record ParseDiagnostic(LineInfo? LineInfo, string Message);

/// <summary>
/// Result of compiling DSL source into a domain model.
/// </summary>
public sealed class DomainModelBuildResult
{
    public DomainModel? Model { get; }
    public IReadOnlyList<ParseDiagnostic> Diagnostics { get; }
    public bool Success => Diagnostics.Count == 0 && Model is not null;

    public DomainModelBuildResult(DomainModel? model, IReadOnlyList<ParseDiagnostic> diagnostics)
    {
        Model = model;
        Diagnostics = diagnostics;
    }
}

/// <summary>
/// Compiles source code into a <see cref="DomainModel"/> object.
/// </summary>
public class DomainModelBuilder
{
    private readonly Logger syntaxLogger = new("Syntax");

    /// <summary>
    /// Run lexing, parsing and generate the model.
    /// </summary>
    /// <param name="src">Source code of the model</param>
    /// <returns>Returns the model generated or null if there were syntax errors.</returns>
    public DomainModel? Run(string src)
    {
        var lexer = CreateLexer(src, out var lexerErrorListener);
        var parser = CreateParser(lexer, out var parserErrorListener);

        var diagnostics = CollectDiagnostics(lexerErrorListener, parserErrorListener);

        DomainModel? model = null;
        if (diagnostics.Count == 0)
            model = GenerateModel(parser);

        var result = new DomainModelBuildResult(model, diagnostics);

        LogResult(result);

        return result.Model;
    }

    private void LogResult(DomainModelBuildResult result)
    {
        if (!result.Success)
        {
            foreach (var diagnostic in result.Diagnostics)
                syntaxLogger.Error(diagnostic.LineInfo, diagnostic.Message);
        }
    }

    private static RestDslLexer CreateLexer(string src, out LexerErrorListener lexerErrorListener)
    {
        var inputStream = new AntlrInputStream(src.ToCharArray(), src.Length);
        var lexer = new RestDslLexer(inputStream);
        lexer.RemoveErrorListeners();
        lexerErrorListener = new LexerErrorListener();
        lexer.AddErrorListener(lexerErrorListener);
        return lexer;
    }

    private static RestDslParser CreateParser(RestDslLexer lexer, out ParserErrorListener parserErrorListener)
    {
        var tokenStream = new CommonTokenStream(lexer);
        var parser = new RestDslParser(tokenStream);
        parser.RemoveErrorListeners();
        parserErrorListener = new ParserErrorListener();
        parser.AddErrorListener(parserErrorListener);
        return parser;
    }

    private static List<ParseDiagnostic> CollectDiagnostics(
        LexerErrorListener lexerErrorListener,
        ParserErrorListener parserErrorListener)
    {
        var diagnostics = new List<ParseDiagnostic>(lexerErrorListener.Errors.Count + parserErrorListener.Errors.Count);

        foreach (var err in lexerErrorListener.Errors)
            diagnostics.Add(new ParseDiagnostic(err.lineInfo, err.msg));

        foreach (var err in parserErrorListener.Errors)
            diagnostics.Add(new ParseDiagnostic(err.lineInfo, err.msg));

        return diagnostics;
    }

    private static DomainModel GenerateModel(RestDslParser parser)
    {
        var context = parser.program();
        var visitor = new VisitorImpl();
        return visitor.VisitProgram(context);
    }
}