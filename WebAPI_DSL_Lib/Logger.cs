using WebAPI_DSL_Lib.Info;

namespace WebAPI_DSL_Lib;

public class Logger(string name)
{
    public bool Suppress { get; set; } = true;
    public enum LogLevels
    {
        Trace, Info, Warn, Error
    }

    /// <summary>
    /// Set log level from a string. Tries to convert <see cref="str"/>
    /// to <see cref="LogLevels"/>.
    /// </summary>
    /// <param name="str"></param>
    /// <exception cref="ArgumentException">If <see cref="str"/> couldn't be converted to <see cref="LogLevels"/></exception>
    public static void SetLogLevelFromString(string str)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            throw new ArgumentException("Log level cannot be null or empty.", nameof(str));
        }

        if (!Enum.TryParse<LogLevels>(str, true, out var parsedLevel))
        {
            throw new ArgumentException($"Invalid log level '{str}'. Valid values: {string.Join(", ", Enum.GetNames<LogLevels>())}.", nameof(str));
        }

        LogLevel = parsedLevel;
    }
    
    /// <summary>
    /// Global log level for all loggers. Messages with a level lower than this value will not be logged.
    /// </summary>
    public static LogLevels LogLevel { get; set; } = LogLevels.Trace;
    private readonly string Name = name;
    private void Write(ConsoleColor color, string prefix, LineInfo? info, string message, LogLevels level)
    {
        if(Suppress) return;
        if(level < LogLevel) return;
        var colorBefore = Console.ForegroundColor;
        var infoAsString = info.HasValue ? info.Value.ToString() : "";
        Console.ForegroundColor = color;
        Console.Error.WriteLine($"{Name} [{prefix}] {infoAsString} | {message}");
        Console.ForegroundColor = colorBefore;
    }
    
    public void Error(LineInfo? lineInfo, string msg)
    {
        Write(ConsoleColor.Red, "ERROR", lineInfo, msg, LogLevels.Error);
    }
    
    public void Error(string msg)
    {
        Write(ConsoleColor.Red, "ERROR", null, msg, LogLevels.Error);
    }
    
    public void Warn(LineInfo? lineInfo, string msg)
    {
        Write(ConsoleColor.DarkYellow, "WARNING", lineInfo, msg, LogLevels.Warn);
    }
    
    public void Warn(string msg)
    {
        Write(ConsoleColor.DarkYellow, "WARNING", null, msg, LogLevels.Warn);
    }

    public void Info(LineInfo? lineInfo, string msg)
    {
        Write(Console.ForegroundColor, "INFO", lineInfo, msg, LogLevels.Info);
    }
    
    public void Info(string msg)
    {
        Write(Console.ForegroundColor, "INFO", null, msg, LogLevels.Info);
    }

    public void Trace(LineInfo? lineInfo, string msg)
    {
        Write(Console.ForegroundColor, "LOG", lineInfo, msg, LogLevels.Trace);
    }
    
    public void Trace(string msg)
    {
        Write(Console.ForegroundColor, "LOG", null, msg, LogLevels.Trace);
    }
}