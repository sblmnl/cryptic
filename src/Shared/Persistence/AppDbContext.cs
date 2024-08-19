using System.Reflection;
using Cryptic.Shared.Features.Notes;
using Microsoft.EntityFrameworkCore;

namespace Cryptic.Shared.Persistence;

public class AppDbContext : DbContext
{
    public required DbSet<DataModels.Note> Notes { get; init; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        base.OnModelCreating(builder);
    }
}
