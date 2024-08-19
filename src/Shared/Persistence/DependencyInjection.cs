using Cryptic.Shared.Features.Notes.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cryptic.Shared.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string dbConnectionString)
    {
        services.AddDbContext<AppDbContext>(o =>
        {
            o.UseNpgsql(dbConnectionString, builder =>
            {
                builder.MigrationsAssembly(AssemblyReference.Assembly.FullName);
            });
            o.UseSnakeCaseNamingConvention();
        });

        services.AddScoped<INoteRepository, NoteRepository>();
        
        return services;
    }
}
