using WebAPI.Features.Notes.CreateNote;
using WebAPI.Features.Notes.DestroyNote;
using WebAPI.Features.Notes.ReadNote;

namespace WebAPI.Features.Notes;

public static class Setup
{
    public static IServiceCollection AddNotes(this IServiceCollection services)
    {
        services.AddScoped<Storage.NoteRepository>();
        
        return services;
    }

    public static void UseNotes(this WebApplication app)
    {
        app.MapCreateNoteEndpoint();
        app.MapReadNoteEndpoint();
        app.MapDestroyNoteEndpoint();
    }
}
