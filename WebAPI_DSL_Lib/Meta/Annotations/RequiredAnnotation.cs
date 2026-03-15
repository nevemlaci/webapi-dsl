using System.Diagnostics;

namespace WebAPI_DSL_Lib.Meta.Annotations;

public class RequiredAnnotation : FieldAnnotation
{
    public override string Name => "@Required";

    public override void Apply(object o, Dictionary<string, object> args)
    {
        base.Apply(o, args);

        Debug.Assert(o is FieldDefinition);
        (o as FieldDefinition)!.IsRequired = true;
    }
}