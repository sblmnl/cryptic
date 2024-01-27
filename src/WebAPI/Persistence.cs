using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WebAPI.Features.Notes;

namespace WebAPI;

public class AppDbContext : DbContext
{
    public required DbSet<Storage.Note> Notes { get; init; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        base.OnModelCreating(builder);
    }
}

public static class DatabaseSetup
{
    public static void MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        dbContext.Database.Migrate();
    }
}
