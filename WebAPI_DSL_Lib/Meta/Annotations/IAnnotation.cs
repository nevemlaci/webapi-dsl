using WebAPI_DSL_Lib.Meta.Annotations.ArgumentHolder;

namespace WebAPI_DSL_Lib.Meta.Annotations;

public interface IAnnotation
{
    public string Name { get; }
    public bool CanApplyTo(object o);

    public void Apply(object o, AnnotationArgumentHolder args);
    
    
}