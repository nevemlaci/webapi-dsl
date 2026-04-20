using Antlr4.Runtime;
using WebAPI_DSL_Lib;
using WebAPI_DSL_Lib.Model;
using WebAPI_DSL_Main.SyntaxErrorHandlers;
using WebAPI_DSL_Main.Visitor;

namespace WebAPI_DSL_Main;

public class DomainModelBuilder
{
    private RestDslLexer lexer;
    private RestDslParser parser;

    private LexerErrorListener lexerErrorListener;
    private ParserErrorListener parserErrorListener;
    
    private Logger syntaxLogger = new("Syntax");

    public DomainModel? Run(string src)
    {
        SetLexer(src);
        SetParser();
        return HandleSyntaxErrors() ? GenerateModel() : null;
    }

    private void SetLexer(string src)
    {
        var inputStream = new AntlrInputStream(src.ToCharArray(), src.Length);
        lexer = new RestDslLexer(inputStream);
        lexer.RemoveErrorListeners();
        lexerErrorListener = new LexerErrorListener();
        lexer.AddErrorListener(lexerErrorListener);
    }

    private void SetParser()
    {
        var tokenStream = new CommonTokenStream(lexer);
        parser = new RestDslParser(tokenStream);
        parser.RemoveErrorListeners();
        parserErrorListener = new ParserErrorListener();
        parser.AddErrorListener(parserErrorListener);
    }

    private bool HandleSyntaxErrors()
    {
        if (lexerErrorListener.Errors.Count > 0 || parserErrorListener.Errors.Count > 0)
        {
            syntaxLogger.Error(null, "Parsing failed with the following errors:");

            foreach (var err in lexerErrorListener.Errors)
                syntaxLogger.Error(err.lineInfo, err.msg);
            
                
            foreach (var err in parserErrorListener.Errors)
                syntaxLogger.Error(err.lineInfo, err.msg);

            return false;
        }
        
        syntaxLogger.Info(null, "Parsing succeeded!");
        return true;
    }

    private DomainModel GenerateModel()
    {
        var context = parser.program();
        var visitor = new VisitorImpl();
        return visitor.VisitProgram(context);
    }
}