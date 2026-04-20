namespace WebAPI_DSL_Lib.Plugins.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ModelAttribute(string name) : NameAttribute(name);