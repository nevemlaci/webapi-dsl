using WebAPI_DSL_Lib.Meta.Annotations.ArgumentProcessing;

namespace WebAPI_DSL_Lib.Meta.Annotations;

public class AnnotationProcessor(string? procName = null)
{
    private Logger logger = new(procName ?? nameof(AnnotationProcessor));
    
    private readonly Dictionary<string, IAnnotation> _annotations = new();

    public void RegisterAnnotation(IAnnotation a)
    {
        _annotations[a.Name] = a;
        logger.Trace($"Registered annotation {a.Name}");
    }
    
    public void RegisterAnnotations(ICollection<IAnnotation> as_)
    {
        foreach (var a in as_)
        {
            RegisterAnnotation(a);
        }
    }
    
    public void ApplyAnnotation(string name, object o, AnnotationArgumentHolder args)
    {
        if (!_annotations.TryGetValue(name, out var annotation))
        {
            logger.Info($"Built-in annotation {name} was not found.");
            return;
        }
        logger.Info($"Applying annotation: {name}");
        annotation.Apply(o, args);
    }
}