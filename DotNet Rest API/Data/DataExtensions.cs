using Microsoft.EntityFrameworkCore;

namespace DotNet_Rest_API.Data
{
    public static class DataExtensions
    {
        public static async Task MigrateDBAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            await dbContext.Database.MigrateAsync();
        }
             
    }
}
