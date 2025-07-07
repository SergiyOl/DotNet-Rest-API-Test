using DotNet_Rest_API.Data;
using DotNet_Rest_API.Endpoints;
using DotNet_Rest_API.Entities;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

//SQLite
var connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSqlite<AppDBContext>(connString);
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

//Identity and Stores
builder.Services
    .AddIdentityApiEndpoints<AppUser>()
    .AddEntityFrameworkStores<AppDBContext>();


//App build
var app = builder.Build();
//Endpoints
app.MapSongsEndpoints();
app.MapGenresEndpoints();
//Migration
await app.MigrateDBAsync();


////App start
app.MapGet("/", () => "Hello World!");
app.Run();
