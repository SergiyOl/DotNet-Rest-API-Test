namespace DotNet_Rest_API.DTOs
{
    public record class SongDto(
        int Id, 
        string Name, 
        string Genre, 
        int Lenght, 
        int Listens
    );
}
