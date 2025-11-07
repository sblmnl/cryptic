using Cryptic.Core.Notes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Cryptic.Core.Persistence;

public class AppDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;

    public virtual DbSet<Note> Notes { get; set; }

    public AppDbContext(
        IConfiguration configuration,
        IHostEnvironment environment,
        DbContextOptions<AppDbContext> options) : base(options)
    {
        _configuration = configuration;
        _environment = environment;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("Database"));
        optionsBuilder.UseSnakeCaseNamingConvention();

        if (_environment.IsDevelopment())
        {
            optionsBuilder.LogTo(Console.WriteLine);
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
    }
}
