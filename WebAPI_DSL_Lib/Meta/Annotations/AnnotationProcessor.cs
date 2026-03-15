namespace WebAPI_DSL_Lib.Meta.Annotations;

public class AnnotationProcessor
{
    public class AnnotationNotFoundException : Exception
    {
        public AnnotationNotFoundException(string name) : base($"Annotation {name} not found!")
        {}
    }
    
    private readonly Dictionary<string, IAnnotation> _annotations = new();

    public void RegisterAnnotation(IAnnotation a)
    {
        _annotations[a.Name] = a;
    }
    
    public void RegisterAnnotations(ICollection<IAnnotation> as_)
    {
        foreach (var a in as_)
        {
            _annotations[a.Name] = a;
        }
    }
    
    public void ApplyAnnotation(string name, object o, Dictionary<string, object> args)
    {
        if (!_annotations.TryGetValue(name, out var annotation))
        {
            return;
        }
        
        annotation.Apply(o, args);
    }
}