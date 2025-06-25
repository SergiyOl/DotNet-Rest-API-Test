using System.ComponentModel.DataAnnotations;

namespace DotNet_Rest_API.DTOs
{
    public record class CreateSongDto(
        [Required][StringLength(50)] string Name,
        [Required][StringLength(20)] string Genre,
        [Required] int Lenght,
        int Listens
    );
}