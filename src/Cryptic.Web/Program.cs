using Cryptic.Core.Features.Notes;
using Cryptic.Core.Persistence;
using Cryptic.Web;
using Cryptic.Web.Common.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();

builder.Services.AddMediatR(c =>
    c.RegisterServicesFromAssembly(Cryptic.Core.AssemblyReference.Assembly));

builder.Services.AddPersistence(builder.Configuration.GetConnectionString("Database")!);
builder.Services.AddNotes();

builder.Services.AddExceptionHandler<ExceptionHandlerMiddleware>();

var app = builder.Build();

app.MigrateDatabase();
app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "api/swagger/{documentName}/swagger.json";
    });
    
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = "api/swagger";
        options.SwaggerEndpoint("/api/swagger/v1/swagger.json", "Cryptic API V1");
    });
}

app.MapEndpoints();
app.UseExceptionHandler(_ => {});
app.MapFallbackToFile("index.html");

app.Run();
