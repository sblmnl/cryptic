using Cryptic.WebAPI;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapEndpoints();

app.Run();
