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
    public static void MigrateDatabase(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (dbContext is null)
        {
            throw new ApplicationException($"Unable to resolve type {nameof(AppDbContext)} from services!");
        }
        
        dbContext.Database.Migrate();
    }
}
