using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace DotNet_Rest_API.DTOs
{
    public record class CreateSongDto(
        [Required][StringLength(50)] string Name,
        [Range(1, 5)] int GenreId,
        [Range(1, Int32.MaxValue)] int Lenght,
        [Range(0, Int32.MaxValue)] int Listens
    );
}