using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace BiometricAttendanceSystem.Infrastrucuture.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> _userManager)
        {
            if (!_userManager.Users.Any())
            {
                var user = new AppUser
                {
                    DisplayName = "agile",
                    Email = "agile@test.com",
                    UserName = "agile",                 
                };

                await _userManager.CreateAsync(user, "Pa$$w0rd");
            }

        }
    }
}
