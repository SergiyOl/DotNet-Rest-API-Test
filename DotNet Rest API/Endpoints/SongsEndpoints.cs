using DotNet_Rest_API.Data;
using DotNet_Rest_API.DTOs;
using DotNet_Rest_API.Entities;

namespace DotNet_Rest_API.Endpoints
{
    public static class SongsEndpoints
    {
        private static readonly List<SongDto> songs = [
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
            group.MapGet("/", () => songs);

            // GET /songs/(id)
            group.MapGet("/{id}", (int id) =>
            { 
                SongDto? song = songs.Find(song => song.Id == id);

                return song is null ? Results.NotFound() : Results.Ok(song);
            })
            .WithName("GetSong");

            // POST /songs
            group.MapPost("/", (CreateSongDto newSong, SongsListContext dbContext) =>
            {
                Song song = new()
                {
                    Name = newSong.Name,
                    Genre = dbContext.Genres.Find(newSong.GenreId),
                    GenreId = newSong.GenreId,
                    Lenght = newSong.Lenght,
                    Listens = newSong.Listens
                };

                dbContext.Songs.Add(song);
                dbContext.SaveChanges();

                SongDto songDto = new(
                    song.Id,
                    song.Name,
                    song.Genre!.Name,
                    song.Lenght,
                    song.Listens
                );

                return Results.CreatedAtRoute("GetSong", new { id = song.Id }, songDto);
            });

            // PUT /songs/(id)
            group.MapPut("/{id}", (int id, UpdateSongDto updatedSong) =>
            //songs.Find(song => song.Id == id)).WithName("GetSong");
            {
                var index = songs.FindIndex(song => song.Id == id);

                if (index == -1)
                {
                    return Results.NotFound();
                }

                songs[index] = new SongDto(
                    id,
                    updatedSong.Name,
                    updatedSong.Genre,
                    updatedSong.Lenght,
                    updatedSong.Listens
                );

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
