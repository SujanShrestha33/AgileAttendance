using Microsoft.AspNetCore.Identity;

namespace Core.Entities.Identity
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public string? Token { get; set; }
        public DateTime? expiresAt { get; set; }
    }
}
