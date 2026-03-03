using System;
using System.Collections.Generic;

namespace ExampleAPI.Generated.Dtos;

public class PostDto
{
    public Guid? Id { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public Guid? AuthorId { get; set; }
}