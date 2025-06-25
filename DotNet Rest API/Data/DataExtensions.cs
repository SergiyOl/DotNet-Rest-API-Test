using Microsoft.EntityFrameworkCore;

namespace DotNet_Rest_API.Data
{
    public static class DataExtensions
    {
        public static void MigrateDB(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SongsListContext>();
            dbContext.Database.Migrate();
        }
             
    }
}
