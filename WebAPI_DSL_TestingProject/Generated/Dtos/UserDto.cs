using System;
using System.Collections.Generic;

namespace ExampleAPI.Generated.Dtos;

public class UserDto
{
    public Guid? Id { get; set; }
    public string? Username { get; set; }
    public ICollection<Guid>? PostsId { get; set; }
}