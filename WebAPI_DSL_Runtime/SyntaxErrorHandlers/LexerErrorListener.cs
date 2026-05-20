namespace WebAPI_DSL_Main.SyntaxErrorHandlers;

using WebAPI_DSL_Lib.Info;

using Antlr4.Runtime;
using System.Collections.Generic;

public class LexerErrorListener : IAntlrErrorListener<int>
{
    public List<(LineInfo lineInfo, string msg)> Errors { get; } = [];

    public void SyntaxError(
        TextWriter output,
        IRecognizer recognizer,
        int offendingSymbol,
        int line,
        int charPositionInLine,
        string msg,
        RecognitionException e)
    {
        LineInfo lineInfo = new(line, charPositionInLine);
        Errors.Add((lineInfo, msg));
    }
}