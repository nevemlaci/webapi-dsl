using ExampleAPI.Generated.DbContext;
using ExampleAPI.Generated.Entities;
using Microsoft.EntityFrameworkCore;

namespace TestingGenerated;

/**
 * Helper class for adding example data to in memor SQLite db
 */
public static class DbInitializer
{
    public static async Task SeedData(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (await context.Users.AnyAsync()) return;

        var user1 = new User { 
            Id = Guid.NewGuid(), 
            Username = "AliceDev",
            Accountage = 5
        };
        var user2 = new User { 
            Id = Guid.NewGuid(), 
            Username = "BobCoder",
            Accountage = 2
        };

        var posts = new List<Post>
        {
            new Post { 
                Id = Guid.NewGuid(), 
                Title = "Getting Started with EF Core", 
                Content = "Entity Framework is a powerful ORM...", 
                Author = user1 
            },
            new Post { 
                Id = Guid.NewGuid(), 
                Title = "SQLite in Memory", 
                Content = "Testing with SQLite is fast and efficient.", 
                Author = user1 
            },
            new Post { 
                Id = Guid.NewGuid(), 
                Title = "C# 12 Features", 
                Content = "Let's talk about primary constructors...", 
                Author = user2 
            }
        };

        // 5. Save to Database
        await context.Users.AddRangeAsync(user1, user2);
        await context.Posts.AddRangeAsync(posts);
        await context.SaveChangesAsync();
    }
}