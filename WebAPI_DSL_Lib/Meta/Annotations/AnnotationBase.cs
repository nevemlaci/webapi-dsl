using WebAPI_DSL_Lib.Meta.Annotations.ArgumentProcessing;

namespace WebAPI_DSL_Lib.Meta.Annotations;

public abstract class AnnotationBase<T> : IAnnotation
{
    public abstract string Name { get; }

    public bool CanApplyTo(object o)
    {
        return o is T;
    }

    public virtual void Apply(object o, AnnotationArgumentHolder args)
    {
        if (!CanApplyTo(o))
        {
            throw new Exception();
        }
    }
}