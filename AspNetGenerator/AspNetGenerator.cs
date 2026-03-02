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

    public AspNetGenerator(DomainModel m)
    {
        model = m;
    }
    
    public void Codegen()
    {
        aspNetModel = new AspNetModel(model);
        GenerateDbContext();
        GenerateControllers();
        GenerateDtos();
        GenerateEntities();
    }

    private void GenerateDbContext()
    {
        var directoryPath = Path.Combine(AppContext.BaseDirectory, "Generated", "DbContext");
        Directory.CreateDirectory(directoryPath);
        var entityTemplate = Template.Parse(File.ReadAllText("Templates/AspDotNet/DbContext.scriban-cs"));
        var templateContext = new TemplateContext
        {
            TemplateLoader = new FileSystemLoader(Path.Combine(AppContext.BaseDirectory, "Templates"))
        };
        var scriptObject = new ScriptObject();
        scriptObject.Import(new
        {
            Namespace = aspNetModel.DbContextNamespace,
            dbContext = aspNetModel.DbContext
        });
        templateContext.PushGlobal(scriptObject);
        var result = entityTemplate.Render(templateContext);
        var filePath = Path.Combine(directoryPath, $"{aspNetModel.DbContext.ClassName}.cs");
        CsCodeChecker.AssertCodeCompiles(filePath, result);
        File.WriteAllText(filePath, result);
        
    }
    
    private void GenerateDtos()
    {
        var directoryPath = Path.Combine(AppContext.BaseDirectory, "Generated", "Dtos");
        Directory.CreateDirectory(directoryPath);
        var entityTemplate = Template.Parse(File.ReadAllText("Templates/AspDotNet/Dto.scriban-cs"));
        
        foreach (var dto in aspNetModel.Dtos)
        {
            var templateContext = new TemplateContext
            {
                TemplateLoader = new FileSystemLoader(Path.Combine(AppContext.BaseDirectory, "Templates"))
            };
            var scriptObject = new ScriptObject();
            scriptObject.Import(new
            {
                Namespace = aspNetModel.DtoNamespace,
                Dto = dto
            });
            templateContext.PushGlobal(scriptObject);
            var result = entityTemplate.Render(templateContext);
            var filePath = Path.Combine(directoryPath, $"{dto.ClassName}.cs");
            CsCodeChecker.AssertCodeCompiles(filePath, result);
            File.WriteAllText(filePath, result);
        }
    }
    
    private void GenerateEntities()
    {
        var directoryPath = Path.Combine(AppContext.BaseDirectory, "Generated", "Entities");
        Directory.CreateDirectory(directoryPath);
        var entityTemplate = Template.Parse(File.ReadAllText("Templates/AspDotNet/Entity.scriban-cs"));
        
        foreach (var entity in aspNetModel.Entities)
        {
            var templateContext = new TemplateContext
            {
                TemplateLoader = new FileSystemLoader(Path.Combine(AppContext.BaseDirectory, "Templates"))
            };
            var scriptObject = new ScriptObject();
            scriptObject.Import(new
            {
                Namespace = aspNetModel.EntityNamespace,
                Entity = entity
            });
            templateContext.PushGlobal(scriptObject);
            var result = entityTemplate.Render(templateContext);
            var filePath = Path.Combine(directoryPath, $"{entity.ClassName}.cs");
            CsCodeChecker.AssertCodeCompiles(filePath, result);
            File.WriteAllText(filePath, result);
        }
    }

    private void GenerateControllers()
    {
        var directoryPath = Path.Combine(AppContext.BaseDirectory, "Generated", "Controllers");
        Directory.CreateDirectory(directoryPath);
        var entityTemplate = Template.Parse(File.ReadAllText("Templates/AspDotNet/Controller.scriban-cs"));
        
        foreach (var controller in aspNetModel.Controllers)
        {
            var templateContext = new TemplateContext
            {
                TemplateLoader = new FileSystemLoader(Path.Combine(AppContext.BaseDirectory, "Templates"))
            };
            var scriptObject = new ScriptObject();
            scriptObject.Import(new
            {
                Namespace = aspNetModel.ControllerNamespace,
                Controller = controller
            });
            templateContext.PushGlobal(scriptObject);
            var result = entityTemplate.Render(templateContext);
            var filePath = Path.Combine(directoryPath, $"{controller.ClassName}.cs");
            CsCodeChecker.AssertCodeCompiles(filePath, result);
            File.WriteAllText(filePath, result);
        }
    }
}