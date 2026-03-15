namespace WebAPI_DSL_Lib.Meta.Annotations;

public class NoDefaultEndpointAnnotation : EntityAnnotation
{
    public override string Name => "@NoDefaultEndpoint";

    public override void Apply(object o, Dictionary<string, object> args)
    {
        base.Apply(o, args);
        if (args.Count > 0)
        {
            throw new IncorrectArgumentCountException(0, args.Count);
        }
        if (o is EntityDefinition entity) entity.GenerateDefaultCrud = false;
    }
}