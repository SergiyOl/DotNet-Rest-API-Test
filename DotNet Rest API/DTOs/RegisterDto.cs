using System.ComponentModel.DataAnnotations;

namespace DotNet_Rest_API.DTOs
{
    public record RegisterDto(
        [Required] string Email,
        [Required] string Password
    );
}
