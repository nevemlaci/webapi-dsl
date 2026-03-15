using System.Diagnostics;

namespace WebAPI_DSL_Lib.Meta.Annotations;

public class UniqueAnnotation : FieldAnnotation
{

    public override string Name => "@Unique";

    public override void Apply(object o, Dictionary<string, object> args)
    {
        base.Apply(o, args);
        
        Debug.Assert(o is FieldDefinition, "object should be FieldDefinition, if this assert failed, something" +
                                           "is wrong with base.CanApplyTo!");
        (o as FieldDefinition)!.IsUnique = true;
    }
}