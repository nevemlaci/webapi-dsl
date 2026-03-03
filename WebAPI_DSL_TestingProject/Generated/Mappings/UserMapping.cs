using System.Linq;
using Mapster;
using ExampleAPI.Generated.Entities;
using ExampleAPI.Generated.Dtos;

namespace ExampleAPI.Generated.Mappings;

public class UserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, UserDto>()
            .Map(dest => dest.PostsId, src => src.Posts.Select(x => x.Id))
;

        config.NewConfig<UserDto, User>()
            .Ignore(dest => dest.Id) 
            .Ignore(dest => dest.Posts)
;
    }
}