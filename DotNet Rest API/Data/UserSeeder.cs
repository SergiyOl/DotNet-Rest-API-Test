using DotNet_Rest_API.Entities;
using Microsoft.AspNetCore.Identity;

namespace DotNet_Rest_API.Data
{
    public static class UserSeeder
    {
        public static async Task SeedUsersAsync(UserManager<AppUser> userManager)
        {
            // Admin user data
            string adminEmail = "admin@example.com";
            string adminPassword = "Admin123!";

            // Check if admin already exists
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser is null)
            {
                // Create admin user
                adminUser = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                // Add admin user
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create admin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }

                // Add roles
                await userManager.AddToRolesAsync(adminUser, new[] { "Admin", "User" });
            }
        }
    }
}
