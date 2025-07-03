using DotNet_Rest_API.Data;
using DotNet_Rest_API.Endpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//SQLite
var connString = builder.Configuration.GetConnectionString("SongsList");
builder.Services.AddSqlite<AppDBContext>(connString);

//Identities
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => 
    options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AppDBContext>();

//App build
var app = builder.Build();
//Endpoints
app.MapSongsEndpoints();
app.MapGenresEndpoints();
//Migration
await app.MigrateDBAsync();

//Auth
app.UseAuthentication();
app.UseAuthorization();

//App start
app.MapGet("/", () => "Hello World!");
app.Run();
