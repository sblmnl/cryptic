using Microsoft.Extensions.DependencyInjection;

namespace Cryptic.Core.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string dbConnectionString)
    {
        services.AddDbContext<AppDbContext>(o =>
        {
            o.Configure(dbConnectionString);
        });
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }
}
