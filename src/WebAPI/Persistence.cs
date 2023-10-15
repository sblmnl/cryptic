using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WebAPI.Features.Notes;

namespace WebAPI;

public interface IAppDbContext
{
    public DbSet<Note> Notes { get; }
    
    public Task<int> SaveChangesAsync(CancellationToken ct);
}

public class AppDbContext : DbContext, IAppDbContext
{
    public required DbSet<Note> Notes { get; init; }

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
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }
}