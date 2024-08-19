using Cryptic.Shared.Persistence;
using Cryptic.WebAPI.Features.Notes;
using Microsoft.EntityFrameworkCore;

namespace Cryptic.WebAPI;

public static class Setup
{
    public static void MapEndpoints(this WebApplication app)
    {
        CreateNote.MapEndpoint(app);
    }
    
    public static void MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        dbContext.Database.Migrate();
    }
}
