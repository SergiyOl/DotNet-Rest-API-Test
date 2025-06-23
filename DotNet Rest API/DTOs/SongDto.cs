namespace DotNet_Rest_API.DTOs
{
    public record class SongDto(int id, string name, string genre, int lenght, int listens)
    {
        int Id;
        string Name;
        string Genre;
        int Lenght;
        int Listens;
    }
}
