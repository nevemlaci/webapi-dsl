using System.Diagnostics;
using Scriban;
using Scriban.Runtime;
using WebAPI_DSL_GeneratorsCommon;
using WebAPI_DSL_Lib;
using WebAPI_DSL_Lib.Generator;
using WebAPI_DSL_Lib.Meta;
using WebAPI_DSL_Lib.Model;

namespace AspNetGenerator;

public class AspNetGenerator : ISourceGenerator<IAspNetModel>
{
    private IAspNetModel aspNetModel;
    private string templateDir;
    private string baseOutputDir;

    public void Codegen(string outputDir, DomainModel domainModel)
    {
        Codegen(outputDir, new AspNetModel(domainModel));
    }

    public void Codegen(string outputDir, IAspNetModel _aspNetModel)
    {
        aspNetModel = _aspNetModel;
        if (aspNetModel is null)
        {
            throw new ArgumentException("Invalid model type!");
        }
        templateDir = Path.Join(AppContext.BaseDirectory, "Templates", "AspDotNet");
        baseOutputDir = Path.Join(outputDir, "Generated");
        GenerateEnums();
        GenerateDbContext();
        GenerateControllers();
        GenerateDtos();
        GenerateEntities();
        GenerateMappings();
    }

    private void CheckAndGenerateFile(string filePath, string content)
    {
        CsCodeChecker.AssertCodeCompiles(filePath, content);
        File.WriteAllText(filePath, content);
    }

    private void GenerateEnums()
    {
        var directoryPath = Path.Join(baseOutputDir, "Enums");
        Directory.CreateDirectory(directoryPath);
        var enumTemplate = Template.Parse(File.ReadAllText(Path.Join(templateDir, "Enums.scriban-cs")));
        
        foreach (var enum_ in aspNetModel.Enums)
        {
            var filePath = Path.Join(directoryPath, $"{enum_.Name}.cs");
            var result = ScribanGeneration.GenerateStringFromTemplate(enumTemplate, filePath, new
            {
                Config = aspNetModel.Config,
                Enum = enum_
            });
            CheckAndGenerateFile(filePath, result);
        }

    }

    private void GenerateDbContext()
    {
        var directoryPath = Path.Join(baseOutputDir, "DbContext");
        Directory.CreateDirectory(directoryPath);
        var dbContextTemplate = Template.Parse(File.ReadAllText(Path.Join(templateDir, "DbContext.scriban-cs")));
        var filePath = Path.Join(directoryPath, $"{aspNetModel.DbContext.ClassName}.cs");
        var result = ScribanGeneration.GenerateStringFromTemplate(dbContextTemplate, filePath, new
        {
            Config = aspNetModel.Config,
            dbContext = aspNetModel.DbContext
        });
        CheckAndGenerateFile(filePath, result);
    }

    private void GenerateDtos()
    {
        var directoryPath = Path.Join(baseOutputDir, "Dtos");
        Directory.CreateDirectory(directoryPath);
        var dtoTemplate = Template.Parse(File.ReadAllText(Path.Join(templateDir, "Dto.scriban-cs")));

        foreach (var dto in aspNetModel.Dtos)
        {
            var filePath = Path.Join(directoryPath, $"{dto.ClassName}.cs");
            var result = ScribanGeneration.GenerateStringFromTemplate(dtoTemplate, filePath, new
            {
                Config = aspNetModel.Config,
                Dto = dto
            });
            CheckAndGenerateFile(filePath, result);
        }
    }

    private void GenerateEntities()
    {
        var directoryPath = Path.Join(baseOutputDir, "Entities");
        Directory.CreateDirectory(directoryPath);
        var entityTemplate = Template.Parse(File.ReadAllText(Path.Join(templateDir, "Entity.scriban-cs")));

        foreach (var entity in aspNetModel.Entities)
        {
            var filePath = Path.Join(directoryPath, $"{entity.ClassName}.cs");
            var result = ScribanGeneration.GenerateStringFromTemplate(entityTemplate, filePath, new
            {
                Config = aspNetModel.Config,
                Entity = entity
            });
            CheckAndGenerateFile(filePath, result);
        }
    }

    private void GenerateMappings()
    {
        var directoryPath = Path.Join(baseOutputDir, "Mappings");
        Directory.CreateDirectory(directoryPath);
        var mappingTemplate = Template.Parse(File.ReadAllText(Path.Join(templateDir, "MapsterConfig.scriban-cs")));

        foreach (var entity in aspNetModel.Entities)
        {
            var filePath = Path.Join(directoryPath, $"{entity.ClassName}Mapping.cs");
            var result = ScribanGeneration.GenerateStringFromTemplate(mappingTemplate, filePath, new
            {
                Config = aspNetModel.Config,
                Entity = entity
            });
            CheckAndGenerateFile(filePath, result);
        }
    }

    private void GenerateControllers()
    {
        var directoryPath = Path.Join(baseOutputDir, "Controllers");
        Directory.CreateDirectory(directoryPath);
        var controllerTemplate = Template.Parse(File.ReadAllText(Path.Join(templateDir, "Controller.scriban-cs")));

        foreach (var controller in aspNetModel.Controllers)
        {
            var filePath = Path.Join(directoryPath, $"{controller.ClassName}.cs");
            var result = ScribanGeneration.GenerateStringFromTemplate(controllerTemplate, filePath, new
            {
                Config = aspNetModel.Config,
                Controller = controller
            });
            CheckAndGenerateFile(filePath, result);
        }
    }
}