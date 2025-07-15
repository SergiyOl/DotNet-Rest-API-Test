using System.ComponentModel.DataAnnotations;

namespace DotNet_Rest_API.DTOs
{
    public record RefreshDto(
        [Required] string RefreshToken
    );
}
