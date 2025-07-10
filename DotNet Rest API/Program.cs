using DotNet_Rest_API.Data;
using DotNet_Rest_API.Endpoints;
using DotNet_Rest_API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

//SQLite
var connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSqlite<AppDBContext>(connString);
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

//Auth services
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("UserOnly", policy => policy.RequireRole("User"))
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

//Identity and Stores
builder.Services
    .AddIdentityApiEndpoints<AppUser>()
    .AddEntityFrameworkStores<AppDBContext>();


//App build
var app = builder.Build();
//Endpoints
app.MapSongsEndpoints();
app.MapGenresEndpoints();
app.MapGroup("account").MapIdentityApi<AppUser>();
//RoleSeeder
var roleManager = app.Services.GetRequiredService<RoleManager<IdentityRole>>();
await RoleSeeder.SeedRolesAsync(roleManager);
//Migration
await app.MigrateDBAsync();
//Auth
app.UseAuthorization();

////App start
app.MapGet("/", () => "Hello World!");
app.Run();
