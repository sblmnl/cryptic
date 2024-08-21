using System.Reflection;
using Cryptic.Shared.Features.Notes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Cryptic.Shared.Persistence;

public class AppDbContext : DbContext
{
    private readonly string _connectionString;
    
    public DbSet<DataModels.Note> Notes { get; init; }

    public AppDbContext(DbContextOptions options) : base(options) { }

    public AppDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        base.OnModelCreating(builder);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.Configure(_connectionString);
        }
    }
}

public static partial class Extensions
{
    public static DbContextOptionsBuilder Configure(
        this DbContextOptionsBuilder optionsBuilder,
        string connectionString)
    {
        optionsBuilder.UseNpgsql(connectionString, builder =>
        {
            builder.MigrationsAssembly(AssemblyReference.Assembly.FullName);
        });
        
        optionsBuilder.UseSnakeCaseNamingConvention();
        
        return optionsBuilder;
    }
}
