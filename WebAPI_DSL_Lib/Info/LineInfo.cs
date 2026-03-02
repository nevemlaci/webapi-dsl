namespace WebAPI_DSL_Lib.Info;

public struct LineInfo(int line, int column)
{
    public int Line { get; } = line;
    public int Column { get; } = column;

    public override string ToString()
    {
        return $"{Line}:{Column}";
    }
}