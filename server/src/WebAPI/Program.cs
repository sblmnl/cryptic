using Cryptic.WebAPI;

const string DB_CONNECTION_STRING_ID = "Postgres";

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var dbConnectionString = app.Configuration.GetConnectionString(DB_CONNECTION_STRING_ID)
    ?? throw new ApplicationException($"Connection string: \"{DB_CONNECTION_STRING_ID}\" not set!");

app.MapRoutes(dbConnectionString);

app.Run();
