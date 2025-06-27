namespace DotNet_Rest_API.Entities
{
    public class Song
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int GenreId { get; set; }
        public Genre? Genre { get; set; }
        public int Lenght { get; set; }
        public int Listens { get; set; }

    }
}
