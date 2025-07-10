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
//Alternative SQLite connection
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

//Auth services
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("UserOnly", policy => policy.RequireRole("User"))
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

//Identity and Stores
builder.Services
    .AddIdentityCore<AppUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddSignInManager<SignInManager<AppUser>>()
    .AddEntityFrameworkStores<AppDBContext>();
builder.Services.AddIdentityApiEndpoints<AppUser>();
//Old Identity and Stores
//builder.Services
//    .AddIdentityApiEndpoints<AppUser>();
//    .AddEntityFrameworkStores<AppDBContext>();


//App build
var app = builder.Build();

//RoleSeederScope
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleSeeder.SeedRolesAsync(roleManager);

    // var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
}

//Endpoints
app.MapSongsEndpoints();
app.MapGenresEndpoints();
app.MapGroup("oldaccount").MapIdentityApi<AppUser>(); // To Delete
app.MapAuthEndpoints();

//Migration
await app.MigrateDBAsync();

//Auth
app.UseAuthorization();

//App start
app.Run();
