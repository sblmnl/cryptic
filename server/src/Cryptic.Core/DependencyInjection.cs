using LiteBus.Commands;
using LiteBus.Extensions.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Cryptic.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>();

        return services;
    }

    public static IServiceCollection AddLiteBusModules(this IServiceCollection services)
    {
        services.AddLiteBus(liteBus =>
        {
            liteBus.AddCommandModule(module =>
            {
                module.RegisterFromAssembly(AssemblyReference.Assembly);
            });
        });

        return services;
    }
}
