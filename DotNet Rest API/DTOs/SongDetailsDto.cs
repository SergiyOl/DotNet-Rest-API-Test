namespace DotNet_Rest_API.DTOs
{
    public record class SongDetailsDto(
        int Id, 
        string Name, 
        int GenreId, 
        int Lenght, 
        int Listens
    );
}
