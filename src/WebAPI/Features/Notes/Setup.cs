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
        app.MapPost("/api/notes", CreateNote.HttpHandler.Handler)
            .WithOpenApi();
        
        app.MapGet("/api/notes/{noteId:guid}", ReadNote.HttpHandler.Handler)
            .WithOpenApi();
    }
}