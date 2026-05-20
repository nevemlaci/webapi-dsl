using WebAPI_DSL_Lib.Meta.Annotations.ArgumentProcessing;

namespace WebAPI_DSL_Lib.Meta.Annotations.Builtin;

public class NoDefaultEndpointAnnotation : EntityAnnotation
{
    public override string Name => "@NoDefaultEndpoint";

    public override void Apply(object o, AnnotationArgumentHolder args)
    {
        base.Apply(o, args);
        args.GetArgs(null);

        if (o is EntityDefinition entity) entity.GenerateDefaultCrud = false;
    }
}