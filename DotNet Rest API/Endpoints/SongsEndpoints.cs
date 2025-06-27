using DotNet_Rest_API.Data;
using DotNet_Rest_API.DTOs;
using DotNet_Rest_API.Entities;
using DotNet_Rest_API.Mapping;
using Microsoft.EntityFrameworkCore;

namespace DotNet_Rest_API.Endpoints
{
    public static class SongsEndpoints
    {
        private static readonly List<SongSummaryDto> songs = [
            new (
                0,
                "song1",
                "Rock",
                186,
                1308
                ),
            new (
                1,
                "song2",
                "Indie",
                168,
                2231
                ),
            new (
                2,
                "song3",
                "Pop",
                191,
                6890
                )
        ];

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
            group.MapDelete("/{id}", (int id) =>
            {
                songs.RemoveAll(song => song.Id == id);

                return Results.NoContent();
            });

            return group;
        }
    }
}
