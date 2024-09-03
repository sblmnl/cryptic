using System.Text;
using Cryptic.Shared.Features.Notes;
using Cryptic.Shared.Persistence;
using Cryptic.WebAPI;
using Cryptic.WebAPI.Common.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();

builder.Services.AddMediatR(c =>
    c.RegisterServicesFromAssembly(Cryptic.Shared.AssemblyReference.Assembly));

builder.Services.AddPersistence(builder.Configuration.GetConnectionString("Database")!);
builder.Services.AddNotes();

builder.Services.AddExceptionHandler<ExceptionHandler>();

var app = builder.Build();

app.MigrateDatabase();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapEndpoints();

app.UseExceptionHandler(_ => {});

app.Run();
