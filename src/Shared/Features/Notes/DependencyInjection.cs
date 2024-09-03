using Microsoft.Extensions.DependencyInjection;

namespace Cryptic.Shared.Features.Notes;

public static class DependencyInjection
{
    public static IServiceCollection AddNotes(this IServiceCollection services)
    {
        services.AddScoped<INoteRepository, NoteRepository>();
        
        return services;
    }
}