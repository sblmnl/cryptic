using Microsoft.EntityFrameworkCore;
using WebAPI;
using WebAPI.Features.Notes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<IAppDbContext, AppDbContext>(o =>
{
    o.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
    o.UseSnakeCaseNamingConvention();
});

builder.Services.AddScoped<INoteRepository, NoteRepository>();

var app = builder.Build();

app.MigrateDatabase();
app.MapEndpoints();

app.Run();
