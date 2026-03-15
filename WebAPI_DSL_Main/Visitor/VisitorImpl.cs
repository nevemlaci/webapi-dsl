using Antlr4.Runtime;
using WebAPI_DSL_Lib;
using WebAPI_DSL_Lib.Info;
using WebAPI_DSL_Lib.Meta;
using WebAPI_DSL_Lib.Meta.Expressions;
using WebAPI_DSL_Lib.Meta.Types;
using WebAPI_DSL_Lib.Model;

namespace WebAPI_DSL_Main.Visitor;

public class VisitorImpl : RestDslBaseVisitor<object>
{
    private LineInfo GetLineInfo(ParserRuleContext ctx)
    {
        return new(ctx.Start.Line, ctx.Start.Column);
    }
    private record TypeInfo(string TypeName, bool IsList);

    public override DomainModel VisitProgram(RestDslParser.ProgramContext context)
    {
        var model = new DomainModel();

        if (context.configBlock() != null)
        {
            var configDict = VisitConfigBlock(context.configBlock());
            model.Config = configDict;
        }

        foreach (var child in context.children)
        {
            switch (child)
            {
                case RestDslParser.EntityContext entityCtx:
                    model.Entities.Add((EntityDefinition)Visit(entityCtx));
                    break;
                case RestDslParser.EnumDeclarationContext enumCtx:
                    model.Enums.Add((EnumDefinition)Visit(enumCtx));
                    break;
            }
        }

        return model;
    }
    
    public override Dictionary<string, string> VisitConfigBlock(RestDslParser.ConfigBlockContext context)
    {
        var dict = new Dictionary<string, string>();
        foreach (var prop in context._configs)
        {
            string key = prop.key.Text;
            string value = prop.value.Text.Trim('"');
            dict[key] = value;
        }
        return dict;
    }
    
    public override object VisitEnumDeclaration(RestDslParser.EnumDeclarationContext context)
    {
        string name = context.name.Text;
        var values = new List<string>();

        foreach (var valToken in context._values)
        {
            values.Add(valToken.Text);
        }

        return new EnumDefinition{ Name = name, Values = values, LineInfo = GetLineInfo(context)};
    }

    public override object VisitEntity(RestDslParser.EntityContext context)
    {
        var entity = new EntityDefinition { Name = context.ID().GetText() };

        foreach (var annotation in context._annotations)
        {
            var txt = annotation.name.Text;
            entity.AnnotationsRaw.Add(((string name, Dictionary<string, object> args))VisitAnnotation(annotation));
        }
        
        foreach (var fieldCtx in context.field())
        {
            entity.Fields.Add(VisitField(fieldCtx));
        }

        entity.LineInfo = GetLineInfo(context);
        
        return entity;
    }

    public override FieldDefinition VisitField(RestDslParser.FieldContext context)
    {
        TypeInfo info = (TypeInfo)Visit(context.typeoffield);

        var field = new FieldDefinition
        {
            Name = context.name.Text,
            RawTypeName = info.TypeName,
            IsList = info.IsList
        };

        foreach (var annotation in context._annotations)
        {
            field.AnnotationsRaw.Add(((string name, Dictionary<string, object> args))VisitAnnotation(annotation));
        }

        if (context.@default != null)
        {
            field.DefaultValue = (IExpression)Visit(context.@default);
        }

        field.LineInfo = GetLineInfo(context);
        
        return field;

    }

    public override object VisitTypeName(RestDslParser.TypeNameContext context)
    {
        if (context.listType() != null)
        {
            var innerTypeCtx = context.listType().typeName();
            var innerTypeInfo = (TypeInfo)Visit(innerTypeCtx);
            
            return innerTypeInfo with { IsList = true };
        }

        if (context.primitiveType() != null)
        {
            return new TypeInfo(context.primitiveType().GetText(), false);
        }

        return new TypeInfo(context.typeid.Text, false);
    }

    public override object VisitAnnotation(RestDslParser.AnnotationContext context)
    {
        var name = context.name.Text;
        Dictionary<string, object> args = new();
        bool namedArgPresent = false;
        if (context.parameterAssignmentTuple() == null) return (name, args);
        for (int i = 0; i < context.parameterAssignmentTuple()._params.Count; ++i)
        {
            var param = context.parameterAssignmentTuple()._params[i];
            var explicitParamName = param.name?.Text;
            if (explicitParamName != null)
            {
                namedArgPresent = true;
            }else if (namedArgPresent)
            {
                Logger.Error(GetLineInfo(param), $"Unnamed parameter not allowed after named " +
                                                 $"parameter was already present!");
                //TODO throw
            }
            var paramname = explicitParamName ?? $"__arg{i+1}";
            var paramvalue = VisitExpression(param.value);
            if (!args.TryAdd(paramname, paramvalue))
            {
                Logger.Error(GetLineInfo(param), $"Duplicate parameter: {paramname}");
                //TODO throw
            }
        }

        return (name, args);
    }

    public override string VisitStringLiteralExpr(RestDslParser.StringLiteralExprContext context)
    {
        return context.value.Text[1..^1];
    }

    public override object VisitIntegerLiteral(RestDslParser.IntegerLiteralContext context)
    {
        return int.Parse(context.value.Text);
    }

    public override object VisitFloatLiteral(RestDslParser.FloatLiteralContext context)
    {
        return double.Parse(context.value.Text);
    }
}



