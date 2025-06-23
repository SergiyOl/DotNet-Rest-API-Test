namespace DotNet_Rest_API.DTOs
{
    public record class UpdateSongDto(
        string Name,
        string Genre,
        int Lenght,
        int Listens
    );
}
