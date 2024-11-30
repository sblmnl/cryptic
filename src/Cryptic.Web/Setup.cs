using Cryptic.Core.Persistence;
using Cryptic.Web.Features.Notes;
using Microsoft.EntityFrameworkCore;

namespace Cryptic.Web;

public static class Setup
{
    public static void MapEndpoints(this WebApplication app)
    {
        CreateNoteEndpoint.Map(app);
        ReadNoteEndpoint.Map(app);
        DestroyNoteEndpoint.Map(app);
    }
    
    public static void MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        dbContext.Database.Migrate();
    }
}
