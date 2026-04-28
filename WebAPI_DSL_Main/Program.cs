using CommandLine;
using WebAPI_DSL_Lib;
using WebAPI_DSL_Lib.Meta.Annotations;
using WebAPI_DSL_Lib.Model;
using WebAPI_DSL_Lib.Resolving;

namespace WebAPI_DSL_Main;

internal static class Program
{
    static void Main(string[] args)
    {
        var logger = new Logger("System");
        
        var filename = "test.restapi";
        var outputDir = AppContext.BaseDirectory;
        var generatorName = string.Empty;
        var modelName = "";
        Parser.Default.ParseArguments<Args>(args)
            .WithParsed(options =>
                {
                    if (options.InputFile != null)
                    {
                        filename = options.InputFile;
                    }

                    if (options.OutputDirectory != null)
                    {
                        outputDir = options.OutputDirectory;
                    }

                    if (options.LogLevel != null)
                    {
                        try
                        {
                            Logger.SetLogLevelFromString(options.LogLevel);
                        }
                        catch (ArgumentException e)
                        {
                            logger.Warn(e.Message);
                            logger.Warn("Falling back to default log level INFO");
                            Logger.LogLevel = Logger.LogLevels.Info;
                        }
                    }
                    else
                    {
                        Logger.LogLevel = Logger.LogLevels.Info;
                    }

                    generatorName = options.Generator;

                    modelName = options.Model;
                }
            );
        string src;
        try
        {
            src = File.ReadAllText(filename);
        }
        catch (Exception e)
        {
            logger.Error(null, $"Couldn't read source file {filename} : {e.Message}");
            return;
        }

        var compiler = new DomainModelBuilder();
        var model = compiler.Run(src);

        if (model is null)
        {
            return;
        }

        var resolver = new Resolver(model, BuiltinAnnotations.CreateDefaultAnnotationProcessor());
        
        var successfulResolution = resolver.Resolve();

        if (!successfulResolution)
        {
            return;
        }
        
        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
            logger.Info(null, $"Created output directory: {outputDir}");
        }

        ModelSelector modelSelector = new();

        var genModel = modelSelector.GetModel(modelName)?.Invoke(model);
        
        GeneratorSelector generatorSelector = new();
        var generator = generatorSelector.GetGenerator(generatorName);
        if (generator is null)
        {
            return;
        }
        
        logger.Info(null, "Starting generation...");
        
        if (genModel is not null)
        {
            logger.Info($"Generating code from model {modelName}");
            generator.Codegen(outputDir, genModel);
        }
        else
        {
            logger.Info("Generating code from DomainModel");
            generator.Codegen(outputDir, model);
        }
        
        logger.Info(null, "Generation successful!");
    }
}