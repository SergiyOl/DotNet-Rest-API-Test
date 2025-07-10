using DotNet_Rest_API.Data;
using DotNet_Rest_API.DTOs;
using DotNet_Rest_API.Entities;
using DotNet_Rest_API.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DotNet_Rest_API.Endpoints
{
    public static class AuthEndpoints
    {
        public static RouteGroupBuilder MapAuthEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("account")
                           .WithParameterValidation();

            // Register
            group.MapPost("/register", async (
                RegisterDto dto,
                UserManager<AppUser> userManager,
                RoleManager<IdentityRole> roleManager) =>
            {
                var user = new AppUser { UserName = dto.Email, Email = dto.Email };
                var result = await userManager.CreateAsync(user, dto.Password);

                if (!result.Succeeded)
                {
                    return Results.BadRequest(result.Errors);
                }

                // Ensure "User" role exists
                if (!await roleManager.RoleExistsAsync("User"))
                {
                    await roleManager.CreateAsync(new IdentityRole("User"));
                }

                // Assign "User" role
                await userManager.AddToRoleAsync(user, "User");

                return Results.Ok("User registered");
            });

            return group;
        }
    }
}

