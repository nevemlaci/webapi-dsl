using WebAPI_DSL_Lib.Info;
using WebAPI_DSL_Lib.Meta;
using WebAPI_DSL_Lib.Meta.Annotations;
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

public class Resolver(DomainModel m, AnnotationProcessor annotationProcessor)
{
    public void Resolve()
    {
        foreach (var e in m.Enums)
        {
            ResolveEnum(e);
        }

        foreach (var e in m.Entities)
        {
            ResolveEntity(e);
        }
    }

    private void Error(LineInfo lineInfo, string error)
    {
        Logger.Error(lineInfo, error);
        throw new ResolverError(error);
    }
    
    private void ResolveEnum(EnumDefinition _enum)
    {
        var isNameUnique = true;
            // model.Enums.All(e => e.Name != _enum.Name) &&
            // model.PrimitiveTypes.All(e => e.Name != _enum.Name);

        if (!isNameUnique)
        {
            Error(_enum.LineInfo, $"Enum name {_enum.Name} is not unique!");
        }
    }
    
    private void ResolveEntity(EntityDefinition entity)
    {
        var isNameUnique = true;
            // model.Enums.All(e => e.Name != entity.Name) &&
            // model.PrimitiveTypes.All(e => e.Name != entity.Name) &&
            // model.Entities.All(e => e.Name != entity.Name);
        
        if (!isNameUnique)
        {
            Error(entity.LineInfo, $"Entity name {entity.Name} is not unique!");
        }

        foreach (var (annotationName, args) in entity.AnnotationsRaw)
        {
            annotationProcessor.ApplyAnnotation(annotationName, entity, args);
        }
        
        foreach (var field in entity.Fields)
        {
            ResolveField(field);
        }
    }

    private void ResolveField(FieldDefinition field)
    {
        var rawTypeName = field.RawTypeName;

        IType? type = m.PrimitiveTypes.Find(rawType => rawType.Name == rawTypeName);

        if (type != null)
        {
            field.Type = type;
        }
        else
        {
            type = m.Enums.FirstOrDefault(e => e.Name == rawTypeName);
            if (type != null)
            {
                field.Type = type;
            }
            else
            {
                type = m.Entities.Find(e => e.Name == rawTypeName);
                if (type != null)
                {
                    field.Type = type;
                }
                else
                {
                    Error(field.LineInfo, $"Unknown type: {rawTypeName}");
                    //TODO throw
                }
            }
        }
        
        foreach (var (annotationName, args) in field.AnnotationsRaw)
        {
            annotationProcessor.ApplyAnnotation(annotationName, field, args);
            ResolveAnnotationArgs(args);
        }
    }

    /// <summary>
    /// Resolves the expressions passed as arguments to annotations.
    /// </summary>
    /// <param name="args"></param>
    private void ResolveAnnotationArgs(AnnotationArgumentHolder args)
    {
        foreach (var (_, argValue) in args)
        {
            ResolveExpression(argValue);
        }
    }

    /// <summary>
    /// Resolves an expression.
    /// </summary>
    /// <param name="e"></param>
    private void ResolveExpression(IExpression e)
    {
        if (e is EnumExpression en)
        {
            en.EnumType = m.Enums.FirstOrDefault(_e => _e.Name == en.RawEnumType);
        }
    }

    private void ProcessAnnotations(EntityDefinition e)
    {
        foreach (var (annotationName, args) in e.AnnotationsRaw)
        {
            annotationProcessor.ApplyAnnotation(annotationName, e, args);
        }

        foreach (var field in e.Fields)
        {
            ProcessFieldAnnotations(field);
        }
    }

    private void ProcessFieldAnnotations(FieldDefinition f)
    {
        foreach (var (annotationName, args) in f.AnnotationsRaw)
        {
            annotationProcessor.ApplyAnnotation(annotationName, f, args);
        }
    }
}