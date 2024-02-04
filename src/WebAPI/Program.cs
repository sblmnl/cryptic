using Microsoft.EntityFrameworkCore;
using WebAPI;
using WebAPI.Features.Notes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();

builder.Services.AddDbContext<AppDbContext>(o =>
{
    o.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
    o.UseSnakeCaseNamingConvention();
});

builder.Services.AddNotes();

var app = builder.Build();

app.MigrateDatabase();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseNotes();

app.Run();
