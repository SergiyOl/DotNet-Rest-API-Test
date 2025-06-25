using DotNet_Rest_API.DTOs;
using DotNet_Rest_API.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapSongsEndpoints();

app.MapGet("/", () => "Hello World!");
app.Run();
