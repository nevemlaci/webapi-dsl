namespace WebAPI_DSL_Lib.Plugins.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class GeneratorAttribute(string name) : NameAttribute(name);