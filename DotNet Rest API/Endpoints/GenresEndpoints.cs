using DotNet_Rest_API.Data;
using DotNet_Rest_API.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace DotNet_Rest_API.Endpoints
{
    public static class GenresEndpoints
    {
        public static RouteGroupBuilder MapGenresEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("genres")
                           .WithParameterValidation();

            // GET /genres
            group.MapGet("/", [Authorize(Policy = "AdminOnly")] async (AppDBContext dbContext) => //Restricting endpoint
                await dbContext.Genres
                   .Select(genre => genre.ToDto())
                   .AsNoTracking()
                   .ToListAsync());

            return group;
        }
    }
}
