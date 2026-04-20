namespace WebAPI_DSL_Lib.Plugins.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public abstract class NameAttribute : Attribute
{
    public string Name { get; private set; }
    public NameAttribute(string name)
    {
        Name = name;
    }
}