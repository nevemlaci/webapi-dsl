using WebAPI_DSL_Lib.Meta.Annotations;
using WebAPI_DSL_Lib.Model;

namespace WebAPI_DSL_Lib.Resolving;

public sealed class ResolverContext(DomainModel model, AnnotationProcessor annotationProcessor, Logger logger)
{
    public DomainModel Model { get; } = model;
    public AnnotationProcessor AnnotationProcessor { get; } = annotationProcessor;
    public Logger Logger { get; } = logger;
}