using DotNet_Rest_API.Data;
using DotNet_Rest_API.DTOs;
using DotNet_Rest_API.Entities;
using DotNet_Rest_API.JWT;
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

            //Login
            group.MapPost("/login", async (
                LoginDto dto,
                UserManager<AppUser> userManager,
                SignInManager<AppUser> signInManager,
                TokenService tokenService,
                IConfiguration config) =>
            {
                // Find user
                var user = await userManager.FindByEmailAsync(dto.Email);
                if (user is null)
                    return Results.BadRequest(new { Error = "Invalid credentials" });

                // Verify password
                var result = await signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);
                if (!result.Succeeded)
                    return Results.BadRequest(new { Error = "Invalid credentials" });

                // Get user`s role
                var roles = await userManager.GetRolesAsync(user);

                // Generate tokens
                var accessToken = tokenService.GenerateAccessToken(user, roles);
                var refreshToken = tokenService.GenerateRefreshToken();
                var refreshExpiry = tokenService.GetRefreshTokenExpiry();

                // Store refresh token
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiry = refreshExpiry;
                await userManager.UpdateAsync(user);

                // Return
                return Results.Ok(new
                {
                    accessToken,
                    expiresIn = int.Parse(config["Jwt:AccessTokenLifetimeMinutes"] ??
                        throw new InvalidOperationException("Jwt:AccessTokenLifetimeMinutes is missing in configuration.")) * 60,
                    refreshToken,
                    refreshTokenExpiresIn = int.Parse(config["Jwt:RefreshTokenLifetimeMinutes"] ??
                        throw new InvalidOperationException("Jwt:RefreshTokenLifetimeMinutes is missing in configuration.")) * 60
                });
            });

            return group;
        }
    }
}

