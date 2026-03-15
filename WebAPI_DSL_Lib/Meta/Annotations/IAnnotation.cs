namespace WebAPI_DSL_Lib.Meta.Annotations;

public interface IAnnotation
{
    public string Name { get; }
    public bool CanApplyTo(object o);

    public void Apply(object o, Dictionary<string, object> args);
    
    
}