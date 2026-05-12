using WebAPI_DSL_Lib.Info;

namespace WebAPI_DSL_Main.SyntaxErrorHandlers;

using Antlr4.Runtime;
using System.Collections.Generic;

public class ParserErrorListener : BaseErrorListener
{
    public List<(LineInfo lineInfo, string msg)> Errors { get; } = [];

    public override void SyntaxError(
        TextWriter output,
        IRecognizer recognizer,
        IToken offendingSymbol,
        int line,
        int charPositionInLine,
        string msg,
        RecognitionException e)
    {
        LineInfo lineInfo = new(line, charPositionInLine);
        Errors.Add((lineInfo, msg));
    }
}