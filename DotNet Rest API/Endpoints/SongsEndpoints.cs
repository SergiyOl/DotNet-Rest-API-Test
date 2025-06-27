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
            group.MapGet("/", (SongsListContext dbContext) => 
                dbContext.Songs
                   .Include(song => song.Genre)
                   .Select(song => song.ToSongSummaryDto())
                   .AsNoTracking());

            // GET /songs/(id)
            group.MapGet("/{id}", (int id, SongsListContext dbContext) =>
            {
                Song? song = dbContext.Songs.Find(id);

                return song is null ? Results.NotFound() : Results.Ok(song.ToSongDetailsDto());
            })
            .WithName("GetSong");

            // POST /songs
            group.MapPost("/", (CreateSongDto newSong, SongsListContext dbContext) =>
            {
                Song song = newSong.ToEntity();

                dbContext.Songs.Add(song);
                dbContext.SaveChanges();

                return Results.CreatedAtRoute("GetSong", new { id = song.Id }, song.ToSongDetailsDto());
            });

            // PUT /songs/(id)
            group.MapPut("/{id}", (int id, UpdateSongDto updatedSong, SongsListContext dbContext) =>
            {
                var existingSong = dbContext.Songs.Find(id);

                if (existingSong is null)
                {
                    return Results.NotFound();
                }

                dbContext.Entry(existingSong).CurrentValues.SetValues(updatedSong.ToEntity(id));
                dbContext.SaveChanges();

                return Results.NoContent();
            });

            // DELETE /songs/(id)
            group.MapDelete("/{id}", (int id, SongsListContext dbContext) =>
            {
                dbContext.Songs
                    .Where(song => song.Id == id)
                    .ExecuteDelete();

                return Results.NoContent();
            });

            return group;
        }
    }
}
