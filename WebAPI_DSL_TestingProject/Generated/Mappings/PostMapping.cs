using System.Linq;
using Mapster;
using ExampleAPI.Generated.Entities;
using ExampleAPI.Generated.Dtos;

namespace ExampleAPI.Generated.Mappings;

public class PostMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Post, PostDto>()
;

        config.NewConfig<PostDto, Post>()
            .Ignore(dest => dest.Id) 
            .Ignore(dest => dest.Author)
;
    }
}