using WebAPI_DSL_Lib.Info;
using WebAPI_DSL_Lib.Meta;
using WebAPI_DSL_Lib.Meta.Annotations.ArgumentHolder;
using WebAPI_DSL_Lib.Meta.Expressions;
using WebAPI_DSL_Lib.Meta.Types;

namespace WebAPI_DSL_Lib.Resolving;

internal static class ResolverHelpers
{
    public static void Error(ResolverContext context, LineInfo lineInfo, string error)
    {
        context.Logger.Error(lineInfo, error);
        throw new ResolverError(error);
    }

    public static void ResolveEnum(ResolverContext context, EnumDefinition @enum)
    {
        context.Logger.Trace($"Resolving enum {@enum.Name}");
        var model = context.Model;
        var isNameUnique = model.Enums.Count(e => e.Name == @enum.Name) <= 1 &&
                           model.Primitives.All(e => e.Name != @enum.Name);

        if (!isNameUnique)
        {
            Error(context, @enum.LineInfo, $"Enum name {@enum.Name} is not unique!");
        }
    }

    public static void ResolveEntity(ResolverContext context, EntityDefinition entity)
    {
        var model = context.Model;
        context.Logger.Trace($"Resolving entity {entity.Name}");
        var isNameUnique = model.Enums.All(e => e.Name != entity.Name) &&
                           model.Primitives.All(e => e.Name != entity.Name) &&
                           model.Entities.Count(e => e.Name == entity.Name) <= 1;

        if (!isNameUnique)
        {
            Error(context, entity.LineInfo, $"Entity name {entity.Name} is not unique!");
        }

        context.Logger.Trace("Checking field name uniqueness!");
        var fieldNames = new HashSet<string>();
        foreach (var field in entity.Fields)
        {
            if (!fieldNames.Add(field.Name))
            {
                Error(context, field.LineInfo, $"Field name {field.Name} is not unique in entity {entity.Name}!");
            }
        }
        context.Logger.Trace("Field names were unique");

        context.Logger.Trace($"Resolving annotation args on entity {entity.Name}");
        foreach (var (annotationName, args) in entity.AnnotationsRaw)
        {
            context.Logger.Trace($"Resolving args of annotation {annotationName}");
            ResolveAnnotationArgs(context, args);
        }

        foreach (var field in entity.Fields)
        {
            ResolveField(context, field);
        }
    }

    public static void ResolveField(ResolverContext context, FieldDefinition field)
    {
        var model = context.Model;
        context.Logger.Trace($"Resolving field {field.Name}");
        var rawTypeName = field.RawTypeName;

        IType? type = model.Primitives.Find(rawType => rawType.Name == rawTypeName);

        if (type != null)
        {
            field.Type = type;
            context.Logger.Trace($"Resolved field type to {type.Name}");
        }
        else
        {
            type = model.Enums.FirstOrDefault(e => e.Name == rawTypeName);
            if (type != null)
            {
                field.Type = type;
                context.Logger.Trace($"Resolved field type to {type.Name}");
            }
            else
            {
                type = model.Entities.Find(e => e.Name == rawTypeName);
                if (type != null)
                {
                    field.Type = type;
                    context.Logger.Trace($"Resolved field type to {type.Name}");
                }
                else
                {
                    Error(context, field.LineInfo, $"Unknown type: {rawTypeName}");
                }
            }
        }

        foreach (var (annotationName, args) in field.AnnotationsRaw)
        {
            context.Logger.Trace($"Processing annotation {annotationName}");
            ResolveAnnotationArgs(context, args);
        }
    }

    public static void ResolveAnnotationArgs(ResolverContext context, AnnotationArgumentHolder args)
    {
        foreach (var (argName, argValue) in args)
        {
            context.Logger.Trace($"Resolving arg {argName}");
            ResolveExpression(context, argValue);
        }
    }

    public static void ResolveExpression(ResolverContext context, IExpression expression)
    {
        context.Logger.Trace("Resolving expression");
        if (expression is EnumExpression enumExpression)
        {
            context.Logger.Trace("Resolving enum expression");
            enumExpression.EnumType = context.Model.Enums.FirstOrDefault(e => e.Name == enumExpression.RawEnumType);
            // TODO Error here if enumExpression.EnumType is null
        }
        context.Logger.Trace($"Resolved value: {expression}");
    }

    public static void ProcessAnnotations(ResolverContext context, EntityDefinition entity)
    {
        context.Logger.Trace($"Processing annotations for entity {entity.Name}");
        foreach (var (annotationName, args) in entity.AnnotationsRaw)
        {
            context.Logger.Trace($"Processing annotation {annotationName}");
            context.AnnotationProcessor.ApplyAnnotation(annotationName, entity, args);
        }

        foreach (var field in entity.Fields)
        {
            ProcessFieldAnnotations(context, field);
        }
    }

    public static void ProcessFieldAnnotations(ResolverContext context, FieldDefinition field)
    {
        context.Logger.Trace($"Processing annotations for field {field.Name}");
        foreach (var (annotationName, args) in field.AnnotationsRaw)
        {
            context.Logger.Trace($"Processing annotation {annotationName}");
            context.AnnotationProcessor.ApplyAnnotation(annotationName, field, args);
        }
    }
}