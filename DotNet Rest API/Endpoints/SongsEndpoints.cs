using DotNet_Rest_API.Data;
using DotNet_Rest_API.DTOs;
using DotNet_Rest_API.Entities;
using DotNet_Rest_API.Mapping;
using Microsoft.EntityFrameworkCore;

namespace DotNet_Rest_API.Endpoints
{
    public static class SongsEndpoints
    {
        public static RouteGroupBuilder MapSongsEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("songs")
                           .WithParameterValidation();

            // GET /songs
            group.MapGet("/", async (SongsListContext dbContext) => 
                await dbContext.Songs
                   .Include(song => song.Genre)
                   .Select(song => song.ToSongSummaryDto())
                   .AsNoTracking()
                   .ToListAsync());

            // GET /songs/(id)
            group.MapGet("/{id}", async (int id, SongsListContext dbContext) =>
            {
                Song? song = await dbContext.Songs.FindAsync(id);

                return song is null ? Results.NotFound() : Results.Ok(song.ToSongDetailsDto());
            })
            .WithName("GetSong");

            // POST /songs
            group.MapPost("/", async (CreateSongDto newSong, SongsListContext dbContext) =>
            {
                Song song = newSong.ToEntity();

                dbContext.Songs.Add(song);
                await dbContext.SaveChangesAsync();

                return Results.CreatedAtRoute("GetSong", new { id = song.Id }, song.ToSongDetailsDto());
            });

            // PUT /songs/(id)
            group.MapPut("/{id}", async (int id, UpdateSongDto updatedSong, SongsListContext dbContext) =>
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

            // DELETE /songs/(id)
            group.MapDelete("/{id}", async (int id, SongsListContext dbContext) =>
            {
                await dbContext.Songs
                    .Where(song => song.Id == id)
                    .ExecuteDeleteAsync();

                return Results.NoContent();
            });

            return group;
        }
    }
}
