using System.Diagnostics;
using Scriban;
using Scriban.Runtime;
using WebAPI_DSL_GeneratorsCommon;
using WebAPI_DSL_Lib;
using WebAPI_DSL_Lib.Meta;
using WebAPI_DSL_Lib.Model;

namespace AspNetGenerator;

public class AspNetGenerator : ISourceGenerator
{
    private DomainModel model;
    private AspNetModel aspNetModel;
    private string templateDir;
    private string baseOutputDir;

    public AspNetGenerator(DomainModel m)
    {
        model = m;
    }
    
    public void Codegen(string outputDir)
    {
        aspNetModel = new AspNetModel(model);
        templateDir = Path.Join(AppContext.BaseDirectory, "Templates", "AspDotNet");
        baseOutputDir = Path.Join(outputDir, "Generated");
        GenerateDbContext(outputDir);
        GenerateControllers(outputDir);
        GenerateDtos(outputDir);
        GenerateEntities(outputDir);
        GenerateMappings();
    }

    private void GenerateDbContext(string outputDir)
    {
        var directoryPath = Path.Join(baseOutputDir, "DbContext");
        Directory.CreateDirectory(directoryPath);
        var entityTemplate = Template.Parse(File.ReadAllText(Path.Join(templateDir, "DbContext.scriban-cs")));
        var templateContext = new TemplateContext
        {
            TemplateLoader = new FileSystemLoader(templateDir)
        };
        var scriptObject = new ScriptObject();
        scriptObject.Import(new
        {
            Config = aspNetModel.Config,
            dbContext = aspNetModel.DbContext
        });
        templateContext.PushGlobal(scriptObject);
        var result = entityTemplate.Render(templateContext);
        var filePath = Path.Join(directoryPath, $"{aspNetModel.DbContext.ClassName}.cs");
        CsCodeChecker.AssertCodeCompiles(filePath, result);
        File.WriteAllText(filePath, result);
        
    }
    
    private void GenerateDtos(string outputDir)
    {
        var directoryPath = Path.Join(baseOutputDir, "Dtos");
        Directory.CreateDirectory(directoryPath);
        var entityTemplate = Template.Parse(File.ReadAllText(Path.Join(templateDir, "Dto.scriban-cs")));
        
        foreach (var dto in aspNetModel.Dtos)
        {
            var templateContext = new TemplateContext
            {
                TemplateLoader = new FileSystemLoader(templateDir)
            };
            var scriptObject = new ScriptObject();
            scriptObject.Import(new
            {
                Config = aspNetModel.Config,
                Dto = dto
            });
            templateContext.PushGlobal(scriptObject);
            var result = entityTemplate.Render(templateContext);
            var filePath = Path.Join(directoryPath, $"{dto.ClassName}.cs");
            CsCodeChecker.AssertCodeCompiles(filePath, result);
            File.WriteAllText(filePath, result);
        }
    }
    
    private void GenerateEntities(string outputDir)
    {
        var directoryPath = Path.Join(baseOutputDir, "Entities");
        Directory.CreateDirectory(directoryPath);
        var entityTemplate = Template.Parse(File.ReadAllText(Path.Join(templateDir, "Entity.scriban-cs")));
        
        foreach (var entity in aspNetModel.Entities)
        {
            var templateContext = new TemplateContext
            {
                TemplateLoader = new FileSystemLoader(templateDir)
            };
            var scriptObject = new ScriptObject();
            scriptObject.Import(new
            {
                Config = aspNetModel.Config,
                Entity = entity
            });
            templateContext.PushGlobal(scriptObject);
            var result = entityTemplate.Render(templateContext);
            var filePath = Path.Join(directoryPath, $"{entity.ClassName}.cs");
            CsCodeChecker.AssertCodeCompiles(filePath, result);
            File.WriteAllText(filePath, result);
        }
    }
    
    private void GenerateMappings()
    {
        var directoryPath = Path.Join(baseOutputDir, "Mappings");
        Directory.CreateDirectory(directoryPath);
        var entityTemplate = Template.Parse(File.ReadAllText(Path.Join(templateDir, "MapsterConfig.scriban-cs")));
        
        foreach (var entity in aspNetModel.Entities)
        {
            var templateContext = new TemplateContext
            {
                TemplateLoader = new FileSystemLoader(templateDir)
            };
            var scriptObject = new ScriptObject();
            scriptObject.Import(new
            {
                Config = aspNetModel.Config,
                Entity = entity
            });
            templateContext.PushGlobal(scriptObject);
            var result = entityTemplate.Render(templateContext);
            var filePath = Path.Join(directoryPath, $"{entity.ClassName}Mapping.cs");
            CsCodeChecker.AssertCodeCompiles(filePath, result);
            File.WriteAllText(filePath, result);
        }
    }

    private void GenerateControllers(string outputDir)
    {
        var directoryPath = Path.Join(baseOutputDir, "Controllers");
        Directory.CreateDirectory(directoryPath);
        var entityTemplate = Template.Parse(File.ReadAllText(Path.Join(templateDir, "Controller.scriban-cs")));
        
        foreach (var controller in aspNetModel.Controllers)
        {
            var templateContext = new TemplateContext
            {
                TemplateLoader = new FileSystemLoader(templateDir)
            };
            var scriptObject = new ScriptObject();
            scriptObject.Import(new
            {
                Config = aspNetModel.Config,
                Controller = controller
            });
            templateContext.PushGlobal(scriptObject);
            var result = entityTemplate.Render(templateContext);
            var filePath = Path.Join(directoryPath, $"{controller.ClassName}.cs");
            CsCodeChecker.AssertCodeCompiles(filePath, result);
            File.WriteAllText(filePath, result);
        }
    }
}