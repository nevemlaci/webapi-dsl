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

    /// <summary>
    /// Logs the result of the domain model build operation.
    /// </summary>
    /// <param name="result">
    /// The <see cref="DomainModelBuildResult"/> containing the model and any diagnostics
    /// produced during compilation.
    /// </param>
    /// <remarks>
    /// If the build was unsuccessful (i.e., <see cref="DomainModelBuildResult.Success"/> is false),
    /// this method iterates through all diagnostics and logs each one as an error using the syntax logger.
    /// If the build was successful, no logging occurs.
    /// </remarks>
    private void LogResult(DomainModelBuildResult result)
    {
        if (!result.Success)
        {
            foreach (var diagnostic in result.Diagnostics)
                syntaxLogger.Error(diagnostic.LineInfo, diagnostic.Message);
        }
    }


    /// <summary>
    /// Create and configure a <see cref="RestDslLexer"/> for the provided DSL source string.
    /// </summary>
    /// <param name="src">The DSL source code to lex.</param>
    /// <param name="lexerErrorListener">
    /// Out parameter that receives the <see cref="LexerErrorListener"/> instance
    /// which collects lexing errors produced while tokenizing <paramref name="src"/>.
    /// </param>
    /// <returns>A configured <see cref="RestDslLexer"/> instance ready for tokenization.</returns>
    /// <remarks>
    /// This method:
    /// - Creates an <see cref="AntlrInputStream"/> from the source characters,
    /// - Constructs a <see cref="RestDslLexer"/> over that stream,
    /// - Removes ANTLR's default error listeners and attaches a custom
    ///   <see cref="LexerErrorListener"/> so callers can later inspect lexing diagnostics.
    /// </remarks>
    private static RestDslLexer CreateLexer(string src, out LexerErrorListener lexerErrorListener)
    {
        var inputStream = new AntlrInputStream(src.ToCharArray(), src.Length);
        var lexer = new RestDslLexer(inputStream);
        lexer.RemoveErrorListeners();
        lexerErrorListener = new LexerErrorListener();
        lexer.AddErrorListener(lexerErrorListener);
        return lexer;
    }

    /// <summary>
    /// Create and configure a <see cref="RestDslParser"/> that reads tokens from the provided lexer.
    /// </summary>
    /// <param name="lexer">A configured <see cref="RestDslLexer"/> that will produce tokens.</param>
    /// <param name="parserErrorListener">
    /// Out parameter that receives the <see cref="ParserErrorListener"/> instance
    /// which collects parsing errors encountered while building the parse tree.
    /// </param>
    /// <returns>A configured <see cref="RestDslParser"/> instance ready to parse the token stream.</returns>
    /// <remarks>
    /// This method wraps the lexer in a <see cref="CommonTokenStream"/>, constructs the parser,
    /// removes ANTLR's default error listeners and attaches a custom <see cref="ParserErrorListener"/>
    /// so callers can later inspect parsing diagnostics.
    /// </remarks>
    private static RestDslParser CreateParser(RestDslLexer lexer, out ParserErrorListener parserErrorListener)
    {
        var tokenStream = new CommonTokenStream(lexer);
        var parser = new RestDslParser(tokenStream);
        parser.RemoveErrorListeners();
        parserErrorListener = new ParserErrorListener();
        parser.AddErrorListener(parserErrorListener);
        return parser;
    }

    /// <summary>
    /// Collects and converts errors reported by the lexer and parser error listeners into
    /// a list of structured <see cref="ParseDiagnostic"/> records.
    /// </summary>
    /// <param name="lexerErrorListener">The listener that collected lexer diagnostics.</param>
    /// <param name="parserErrorListener">The listener that collected parser diagnostics.</param>
    /// <returns>
    /// A list of <see cref="ParseDiagnostic"/> instances containing a <see cref="LineInfo"/> (when available)
    /// and a human-readable message. Lexer diagnostics are added first, followed by parser diagnostics.
    /// </returns>
    /// <remarks>
    /// The resulting list is pre-sized by adding the counts of errors from both listeners to avoid
    /// repeated resizing when populating the list.
    /// </remarks>
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

    /// <summary>
    /// Generates a <see cref="DomainModel"/> from the parse tree produced by the parser.
    /// </summary>
    /// <param name="parser">
    /// A configured <see cref="RestDslParser"/> instance that has already parsed the DSL source code.
    /// </param>
    /// <returns>
    /// A <see cref="DomainModel"/> instance constructed from the parse tree using the <see cref="VisitorImpl"/>.
    /// </returns>
    /// <remarks>
    /// This method extracts the program context from the parser and uses the <see cref="VisitorImpl"/>
    /// visitor pattern to traverse the abstract syntax tree (AST) and construct the corresponding
    /// domain model representation.
    /// </remarks>
    private static DomainModel GenerateModel(RestDslParser parser)
    {
        var context = parser.program();
        var visitor = new VisitorImpl();
        return visitor.VisitProgram(context);
    }
}