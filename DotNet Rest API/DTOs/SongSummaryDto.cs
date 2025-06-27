namespace DotNet_Rest_API.DTOs
{
    public record class SongSummaryDto(
        int Id, 
        string Name, 
        string Genre, 
        int Lenght, 
        int Listens
    );
}
