using WebAPI_DSL_Lib.Info;
using WebAPI_DSL_Lib.Meta;
using WebAPI_DSL_Lib.Meta.Annotations;
using WebAPI_DSL_Lib.Meta.Annotations.ArgumentHolder;
using WebAPI_DSL_Lib.Meta.Expressions;
using WebAPI_DSL_Lib.Meta.Types;

namespace WebAPI_DSL_Lib.Model;

public class ResolverError : Exception
{
    
    public ResolverError()
    {
    }

    public ResolverError(string message) : base(message)
    {
    }

    public ResolverError(string message, Exception inner) : base(message, inner)
    {
    }
}

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
        bool success = true;
        foreach (var e in m.Enums)
        {
            try
            {
                ResolveEnum(e);
            }
            catch (ResolverError)
            {
                success = false;
            }
        }

        foreach (var e in m.Entities)
        {
            try
            {
                ResolveEntity(e);
            }
            catch (ResolverError)
            {
                success = false;
            }
        }
        
        foreach (var e in m.Entities)
        {
            try
            {
                ProcessAnnotations(e);
            }
            catch (ResolverError)
            {
                success = false;
            }
        }

        if (success)
        {
            logger.Info(null, "Resolving succesful!");
        }
        
        return success;
    }

    private void Error(LineInfo lineInfo, string error)
    {
        logger.Error(lineInfo, error);
        throw new ResolverError(error);
    }
    
    /// <summary>
    /// Resolve an enum.
    /// Duplicate enum names are not allowed.
    /// </summary>
    /// <param name="_enum"></param>
    private void ResolveEnum(EnumDefinition _enum)
    {
        logger.Trace($"Resolving enum {_enum.Name}");
        var isNameUnique = m.Enums.Count(e => e.Name == _enum.Name) <= 1 &&
                           m.Primitives.All(e => e.Name != _enum.Name);

        if (!isNameUnique)
        {
            Error(_enum.LineInfo, $"Enum name {_enum.Name} is not unique!");
        }
    }
    
    /// <summary>
    /// Resolve an entity.
    /// Duplicate entity names and entity names matching an
    /// already defined enum name are not allowed.
    /// </summary>
    /// <param name="entity"></param>
    private void ResolveEntity(EntityDefinition entity)
    {
        logger.Trace($"Resolving entity {entity.Name}");
        var isNameUnique = m.Enums.All(e => e.Name != entity.Name) &&
                           m.Primitives.All(e => e.Name != entity.Name) &&
                           m.Entities.Count(e => e.Name == entity.Name) <= 1;
        
        if (!isNameUnique)
        {
            Error(entity.LineInfo, $"Entity name {entity.Name} is not unique!");
        }
        
        logger.Trace("Checking field name uniqueness!");
        var fieldNames = new HashSet<string>();
        foreach (var field in entity.Fields)
        {
            if (!fieldNames.Add(field.Name))
            {
                Error(field.LineInfo, $"Field name {field.Name} is not unique in entity {entity.Name}!");
            }
        }
        logger.Trace("Field names were unique");

        logger.Trace($"Resolving annotation args on entity {entity.Name}");
        foreach (var (annotationName, args) in entity.AnnotationsRaw)
        {
            logger.Trace($"Resolving args of annotation {annotationName}");
            ResolveAnnotationArgs(args);
        }
        
        foreach (var field in entity.Fields)
        {
            ResolveField(field);
        }
    }

    /// <summary>
    /// Resolves a field
    /// </summary>
    /// <param name="field">The field definition to resolve.</param>
    private void ResolveField(FieldDefinition field)
    {
        logger.Trace($"Resolving field {field.Name}");
        var rawTypeName = field.RawTypeName;

        IType? type = m.Primitives.Find(rawType => rawType.Name == rawTypeName);

        if (type != null)
        {
            field.Type = type;
            logger.Trace($"Resolved field type to {type.Name}");
        }
        else
        {
            type = m.Enums.FirstOrDefault(e => e.Name == rawTypeName);
            if (type != null)
            {
                field.Type = type;
                logger.Trace($"Resolved field type to {type.Name}");
            }
            else
            {
                type = m.Entities.Find(e => e.Name == rawTypeName);
                if (type != null)
                {
                    field.Type = type;
                    logger.Trace($"Resolved field type to {type.Name}");
                }
                else
                {
                    Error(field.LineInfo, $"Unknown type: {rawTypeName}");
                }
            }
        }
        
        foreach (var (annotationName, args) in field.AnnotationsRaw)
        {
            logger.Trace($"Processing annotation {annotationName}");
            ResolveAnnotationArgs(args);
        }
    }

    /// <summary>
    /// Resolves the expressions passed as arguments to annotations.
    /// </summary>
    /// <param name="args"></param>
    private void ResolveAnnotationArgs(AnnotationArgumentHolder args)
    {
        foreach (var (argName, argValue) in args)
        {
            logger.Trace($"Resolving arg {argName}");
            ResolveExpression(argValue);
        }
    }

    /// <summary>
    /// Resolves an expression.
    /// </summary>
    /// <param name="e"></param>
    private void ResolveExpression(IExpression e)
    {
        logger.Trace($"Resolving expression");
        if (e is EnumExpression en)
        {
            logger.Trace("Resolving enum expression");
            en.EnumType = m.Enums.FirstOrDefault(_e => _e.Name == en.RawEnumType);
            //TODO Error here if en.EnumType is null
        }
        logger.Trace($"Resolved value: {e}");
    }

    private void ProcessAnnotations(EntityDefinition e)
    {
        logger.Trace($"Processing annotations for entity {e.Name}");
        foreach (var (annotationName, args) in e.AnnotationsRaw)
        {
            logger.Trace($"Processing annotation {annotationName}");
            annotationProcessor.ApplyAnnotation(annotationName, e, args);
        }

        foreach (var field in e.Fields)
        {
            
            ProcessFieldAnnotations(field);
        }
    }

    private void ProcessFieldAnnotations(FieldDefinition f)
    {
        logger.Trace($"Processing annotations for field {f.Name}");
        foreach (var (annotationName, args) in f.AnnotationsRaw)
        {
            logger.Trace($"Processing annotation {annotationName}");
            annotationProcessor.ApplyAnnotation(annotationName, f, args);
        }
    }
}