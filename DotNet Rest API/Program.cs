using DotNet_Rest_API.Data;
using DotNet_Rest_API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

//SQLite
var connString = builder.Configuration.GetConnectionString("SongsList");
builder.Services.AddSqlite<SongsListContext>(connString);

var app = builder.Build();
app.MapSongsEndpoints();

await app.MigrateDBAsync();

app.MapGet("/", () => "Hello World!");
app.Run();
