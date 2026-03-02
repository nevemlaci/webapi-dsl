using System.Drawing;
using WebAPI_DSL_Lib.Info;

namespace WebAPI_DSL_Lib;

public static class Logger
{
    private static void Write(ConsoleColor color, LineInfo info, string message)
    {
        var colorBefore = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Error.WriteLine($"{info} | {message}");
        Console.ForegroundColor = colorBefore;
    }
    
    public static void Error(LineInfo lineInfo, string msg)
    {
        Write(ConsoleColor.Red, lineInfo, msg);
    }
    
    public static void Warn(LineInfo lineInfo, string msg)
    {
        Write(ConsoleColor.DarkYellow, lineInfo, msg);
    }

    public static void Info(LineInfo lineInfo, string msg)
    {
        Write(Console.ForegroundColor, lineInfo, msg);
    }
}