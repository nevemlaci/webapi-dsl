using Microsoft.EntityFrameworkCore;
using ExampleAPI.Generated.Entities;

namespace ExampleAPI.Generated.DbContext;

public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(e => e.Username)
            .IsUnique();

        modelBuilder.Entity<Post>()
            .HasOne(d => d.Author)
            .WithMany(p => p.Posts)
            .HasForeignKey(d => d.AuthorId);
    }
}