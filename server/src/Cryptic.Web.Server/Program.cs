using Cryptic.Core;
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistence();
builder.Services.AddLiteBusModules();

builder.Services.Configure<JsonOptions>(o =>
{
    o.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    o.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    o.SerializerOptions.AllowTrailingCommas = true;
});

builder.Services.AddExceptionHandler<ExceptionHandlerMiddleware>();
builder.Services.AddOpenApi();

var app = builder.Build();
string? pathBase = app.Configuration.GetValue<string>("PathBase");

if (!string.IsNullOrWhiteSpace(pathBase))
{
    app.UsePathBase(pathBase);
    app.UseRouting();
}

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapHttpEndpoints();

app.MapFallbackToFile("index.html", new StaticFileOptions
{
    OnPrepareResponse = context =>
    {
        var responseHeaders = context.Context.Response.Headers;
        responseHeaders.CacheControl = "no-cache, no-store, must-revalidate";
        responseHeaders.Pragma = "no-cache";
        responseHeaders.Expires = "0";
    }
});

app.UseExceptionHandler(_ => {});

await app.RunAsync();
