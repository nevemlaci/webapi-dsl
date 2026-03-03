using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExampleAPI.Generated.Entities;

public class Post
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public Guid AuthorId { get; set; }

    [ForeignKey("AuthorId")]
    public virtual User Author { get; set; }
}