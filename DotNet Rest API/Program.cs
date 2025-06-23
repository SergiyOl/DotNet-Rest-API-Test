using DotNet_Rest_API.DTOs;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

List<SongDto> songs = [
    new (
        1,
        "song1",
        "Rock",
        186,
        1308
        ),
    new (
        2,
        "song2",
        "Indie",
        168,
        2231
        ),
    new (
        3,
        "song3",
        "Pop",
        191,
        6890
        )
];

app.MapGet("/", () => "Hello World!");

app.Run();
