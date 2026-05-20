using System.Collections.Immutable;
using AspNetGenerator.SourceDataClasses;
using WebAPI_DSL_Lib.Plugins;

namespace AspNetGenerator;

public interface IAspNetModel
{
    public record ModelConfig(
        string DbContextNamespace,
        string EntityNamespace,
        string ControllerNamespace,
        string DtoNamespace,
        string MappingNamespace,
        string EnumNamespace
    );
    
    public ModelConfig Config { get; }
    
    public IImmutableList<ControllerSource> Controllers {get;}
    public IImmutableList<DtoSource> Dtos {get;}
    public IImmutableList<EntitySource> Entities {get;}
    public IImmutableList<EnumSource> Enums {get;}
    public DbContextSource DbContext {get;}
}