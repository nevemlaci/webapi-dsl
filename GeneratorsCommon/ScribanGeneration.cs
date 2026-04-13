using Scriban;
using Scriban.Runtime;

namespace WebAPI_DSL_GeneratorsCommon;

public static class ScribanGeneration
{
    public static string GenerateStringFromTemplate(Template template, string outputFile, object o)
    {
        var templateContext = new TemplateContext();
        var scriptObject = new ScriptObject();
        scriptObject.Import(o);
        templateContext.PushGlobal(scriptObject);
        var result = template.Render(templateContext);

        return result;
    }
}