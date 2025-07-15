using DotNet_Rest_API.Data;
using DotNet_Rest_API.Data.Migrations;
using DotNet_Rest_API.DTOs;
using DotNet_Rest_API.Entities;
using DotNet_Rest_API.JWT;
using DotNet_Rest_API.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
                // Create user
                var user = new AppUser { UserName = dto.Email, Email = dto.Email };
                var result = await userManager.CreateAsync(user, dto.Password);

                // Creation failure
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

                // Return
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

            //Refresh
            group.MapPost("/refresh", async (
                RefreshDto dto,
                UserManager<AppUser> userManager,
                TokenService tokenService,
                IConfiguration config) =>
            {
                // Find user with this refresh token
                var user = await userManager.Users
                    .Where(u => u.RefreshToken == dto.RefreshToken)
                    .FirstOrDefaultAsync();
                
                // Check token existance and expiary date
                if (user is null || user.RefreshTokenExpiry < DateTime.UtcNow)
                    return Results.Unauthorized();
                
                // Get User`s roles
                var roles = await userManager.GetRolesAsync(user);

                // Generate new tokens
                var newAccessToken = tokenService.GenerateAccessToken(user, roles);
                var newRefreshToken = tokenService.GenerateRefreshToken();
                var newRefreshExpiry = tokenService.GetRefreshTokenExpiry();

                // Update stored refresh token
                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiry = newRefreshExpiry;
                await userManager.UpdateAsync(user);

                // Return
                return Results.Ok(new
                {
                    accessToken = newAccessToken,
                    expiresIn = int.Parse(config["Jwt:AccessTokenLifetimeMinutes"] ??
                        throw new InvalidOperationException("Jwt:AccessTokenLifetimeMinutes is missing in configuration.")) * 60,
                    refreshToken = newRefreshToken,
                    refreshTokenExpiresIn = int.Parse(config["Jwt:RefreshTokenLifetimeMinutes"] ??
                        throw new InvalidOperationException("Jwt:RefreshTokenLifetimeMinutes is missing in configuration.")) * 60
                });
            });

            // Logout
            group.MapPost("/logout", [Authorize(Policy = "UserOnly")] async (
                ClaimsPrincipal userPrincipal,
                UserManager<AppUser> userManager) =>
            {
                // Find user
                var user = await userManager.GetUserAsync(userPrincipal);
                if (user is null)
                    return Results.Unauthorized();

                // Delete refresh token
                user.RefreshToken = null;
                user.RefreshTokenExpiry = null;
                await userManager.UpdateAsync(user);

                // Return
                return Results.Ok(new { message = "Logged out successfully" });
            });

            return group;
        }
    }
}

