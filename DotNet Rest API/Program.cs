using DotNet_Rest_API.Data;
using DotNet_Rest_API.Endpoints;
using DotNet_Rest_API.Entities;
using DotNet_Rest_API.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//SQLite
var connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSqlite<AppDBContext>(connString);
//Alternative SQLite connection
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

//Authorization and policies
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("UserOnly", policy => policy.RequireRole("User"))
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

//Authentication and JWT
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration.GetSection("Jwt");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            //ValidIssuer = jwt["Issuer"],
            //ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt["Key"] ?? 
                throw new InvalidOperationException("Jwt:Key is missing in configuration.")))
        };
    });

//Identity and Stores
builder.Services
    .AddIdentityCore<AppUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddSignInManager<SignInManager<AppUser>>()
    .AddEntityFrameworkStores<AppDBContext>();

//Old auth endpoints
//builder.Services.AddIdentityApiEndpoints<AppUser>();

//Old Identity and Stores
//builder.Services
//    .AddIdentityApiEndpoints<AppUser>();
//    .AddEntityFrameworkStores<AppDBContext>();

//JWT
builder.Services.AddScoped<TokenService>();

//App build
var app = builder.Build();

//RoleSeederScope
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleSeeder.SeedRolesAsync(roleManager);

    // var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
}

//Auth
app.UseAuthentication();
app.UseAuthorization();

//Endpoints
app.MapSongsEndpoints();
app.MapGenresEndpoints();
app.MapAuthEndpoints();
//app.MapGroup("oldaccount").MapIdentityApi<AppUser>(); // To Delete

//DB Migration
await app.MigrateDBAsync();

//App start
app.Run();
