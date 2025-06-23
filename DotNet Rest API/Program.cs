using DotNet_Rest_API.DTOs;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

List<SongDto> songs = [
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

// GET /songs
app.MapGet("songs", () => songs);

// GET /songs/(id)
app.MapGet("songs/{id}", (int id) =>
{
    SongDto? song = songs.Find(song => song.Id == id);

    return song is null ? Results.NotFound() : Results.Ok(song);
})
.WithName("GetSong");

// POST /songs
app.MapPost("songs", (CreateSongDto newSong) =>
{
    SongDto song = new(
        songs.Count,
        newSong.Name,
        newSong.Genre,
        newSong.Lenght,
        newSong.Listens);

    songs.Add(song);

    return Results.CreatedAtRoute("GetSong", new { id = song.Id }, song);
});

// PUT /songs/(id)
app.MapPut("songs/{id}", (int id, UpdateSongDto updatedSong) =>
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
app.MapDelete("songs/{id}", (int id) =>
{
    songs.RemoveAll(song => song.Id == id);

    return Results.NoContent();
});



app.MapGet("/", () => "Hello World!");
app.Run();
