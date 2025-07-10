using DotNet_Rest_API.Data;
using DotNet_Rest_API.DTOs;
using DotNet_Rest_API.Entities;
using DotNet_Rest_API.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace DotNet_Rest_API.Endpoints
{
    [Authorize(Policy = "UserOnly")]
    public static class SongsEndpoints
    {
        public static RouteGroupBuilder MapSongsEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("songs")
                           .RequireAuthorization("UserOnly") //Restricting group
                           .WithParameterValidation();

            // GET /songs
            group.MapGet("/", async (AppDBContext dbContext) => 
                await dbContext.Songs
                   .Include(song => song.Genre)
                   .Select(song => song.ToSongSummaryDto())
                   .AsNoTracking()
                   .ToListAsync());

            // GET /songs/(id)
            group.MapGet("/{id}", async (int id, AppDBContext dbContext) =>
            {
                Song? song = await dbContext.Songs.FindAsync(id);

                return song is null ? Results.NotFound() : Results.Ok(song.ToSongDetailsDto());
            })
            .WithName("GetSong");

            // POST /songs
            group.MapPost("/", async (CreateSongDto newSong, AppDBContext dbContext) =>
            {
                Song song = newSong.ToEntity();

                dbContext.Songs.Add(song);
                await dbContext.SaveChangesAsync();

                return Results.CreatedAtRoute("GetSong", new { id = song.Id }, song.ToSongDetailsDto());
            });
            //.RequireAuthorization();

            // PUT /songs/(id)
            group.MapPut("/{id}", async (int id, UpdateSongDto updatedSong, AppDBContext dbContext) =>
            {
                var existingSong = await dbContext.Songs.FindAsync(id);

                if (existingSong is null)
                {
                    return Results.NotFound();
                }

                dbContext.Entry(existingSong).CurrentValues.SetValues(updatedSong.ToEntity(id));
                await dbContext.SaveChangesAsync();

                return Results.NoContent();
            });
            //.RequireAuthorization();

            // DELETE /songs/(id)
            group.MapDelete("/{id}", async (int id, AppDBContext dbContext) =>
            {
                await dbContext.Songs
                    .Where(song => song.Id == id)
                    .ExecuteDeleteAsync();

                return Results.NoContent();
            });
            //.RequireAuthorization();

            return group;
        }
    }
}
