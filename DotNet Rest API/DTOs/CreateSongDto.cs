namespace DotNet_Rest_API.DTOs
{
    public record class CreateSongDto(
        string Name,
        string Genre,
        int Lenght,
        int Listens
    );
}