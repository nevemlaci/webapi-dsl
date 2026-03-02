using CommandLine;

namespace WebAPI_DSL_Main;

public class Args
{
    [Option('i', "input", Required = false)]
    public string? InputFile { get; set; }
}

