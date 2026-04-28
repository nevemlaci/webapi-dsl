using WebAPI_DSL_Lib.Meta.Annotations;
using WebAPI_DSL_Lib.Model;

namespace WebAPI_DSL_Lib.Resolving;

/// <summary>
/// The resolving stage is responsible for connecting the parts of the model
/// based on the raw names stored inside the model object.
///
/// This usually involves finding the types of fields and expressions based
/// on raw names and sometimes checking for duplicate/erroneous names.
/// </summary>
/// <param name="m"></param>
/// <param name="annotationProcessor"></param>
public class Resolver(DomainModel m, AnnotationProcessor annotationProcessor)
{
    private readonly Logger logger = new("Resolver");

    public bool Resolve()
    {
        var context = new ResolverContext(m, annotationProcessor, logger);
        var pipeline = new ResolverPipeline([
            new EnumValidationStage(),
            new EntityValidationStage(),
            new AnnotationProcessingStage()
        ]);

        var success = pipeline.Execute(context);

        if (success)
        {
            logger.Info(null, "Resolving succesful!");
        }

        return success;
    }
}