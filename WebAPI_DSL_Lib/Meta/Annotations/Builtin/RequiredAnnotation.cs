using System.Diagnostics;
using WebAPI_DSL_Lib.Meta.Annotations.ArgumentProcessing;

namespace WebAPI_DSL_Lib.Meta.Annotations.Builtin;

public class RequiredAnnotation : FieldAnnotation
{
    public override string Name => "@Required";

    public override void Apply(object o, AnnotationArgumentHolder args)
    {
        base.Apply(o, args);
        args.GetArgs(null);

        Debug.Assert(o is FieldDefinition);
        (o as FieldDefinition)!.IsRequired = true;
    }
}