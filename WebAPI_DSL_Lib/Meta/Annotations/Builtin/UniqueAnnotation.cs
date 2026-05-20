using System.Diagnostics;
using WebAPI_DSL_Lib.Meta.Annotations.ArgumentProcessing;

namespace WebAPI_DSL_Lib.Meta.Annotations.Builtin;

public class UniqueAnnotation : FieldAnnotation
{

    public override string Name => "@Unique";

    public override void Apply(object o, AnnotationArgumentHolder args)
    {
        base.Apply(o, args);
        args.GetArgs(null);
        Debug.Assert(o is FieldDefinition, "object should be FieldDefinition, if this assert failed, something" +
                                           "is wrong with base.CanApplyTo!");
        (o as FieldDefinition)!.IsUnique = true;
    }
}