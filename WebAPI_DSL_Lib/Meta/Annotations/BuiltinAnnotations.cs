namespace WebAPI_DSL_Lib.Meta.Annotations;

public static class BuiltinAnnotations
{
    private static readonly IAnnotation NoDefaultEndpoint = new NoDefaultEndpointAnnotation();
    private static readonly IAnnotation RouteAnnotation = new RouteAnnotation();
    private static readonly IAnnotation UniqueAnnotation = new UniqueAnnotation();
    private static readonly IAnnotation RequiredAnnotation = new RequiredAnnotation();
    private static readonly IAnnotation FilterAnnotation = new FilterAnnotation();

    public static AnnotationProcessor CreateDefaultAnnotationProcessor()
    {
        var processor = new AnnotationProcessor();
        processor.RegisterAnnotations(
            [
                NoDefaultEndpoint,
                RouteAnnotation,
                UniqueAnnotation,
                RequiredAnnotation,
                FilterAnnotation
            ]
        );

        return processor;
    }
}