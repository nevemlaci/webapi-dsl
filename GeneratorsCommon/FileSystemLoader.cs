using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;

namespace WebAPI_DSL_GeneratorsCommon;

public class FileSystemLoader(string basePath) : ITemplateLoader
{
    public string GetPath(TemplateContext context, SourceSpan span, string templateName)
    {
        return Path.Combine(basePath, templateName);
    }

    public string Load(TemplateContext context, SourceSpan span, string templatePath)
    {
        return File.ReadAllText(templatePath);
    }

    public async ValueTask<string> LoadAsync(TemplateContext context, SourceSpan span, string templatePath)
    {
        return await File.ReadAllTextAsync(templatePath);
    }
}