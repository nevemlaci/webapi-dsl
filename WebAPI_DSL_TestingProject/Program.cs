using ExampleAPI.Generated.DbContext;
using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TestingGenerated;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var keepAliveConnection = new SqliteConnection("DataSource=:memory:");
        keepAliveConnection.Open();
        
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite(keepAliveConnection);
        });
        
        builder.Services.AddControllers();

        TypeAdapterConfig.GlobalSettings.Scan(typeof(Program).Assembly);
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        var app = builder.Build();
        
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.EnsureCreatedAsync();
    
            await DbInitializer.SeedData(db);
        }
        
        app.UseSwagger();
        app.UseSwaggerUI();
        
        app.UseHttpsRedirection();
        app.UseAuthorization();
        
        app.MapControllers();
        
        await app.RunAsync();
    }
}