using CommandLine;

namespace WebAPI_DSL_Main;

public class Args
{
    [Option('i', "input", Required = false)]
    public string? InputFile { get; set; }
    
    [Option('o', "output", Required = false)]
    public string? OutputDirectory { get; set; }
    
    [Option('g', "generator", Required = true)]
    public string? Generator { get; set; }
    
    [Option('m', "model", Required = false)]
    public string? Model { get; set; }
    
    [Option('l', "loglevel", Required = false)]
    public string? LogLevel { get; set; } 
}

