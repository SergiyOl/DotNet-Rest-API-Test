using Microsoft.AspNetCore.Identity;

namespace DotNet_Rest_API.Entities
{
    public class AppUser : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
    }
}
