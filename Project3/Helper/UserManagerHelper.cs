using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BiometricAttendanceSystem.Helper
{
    public static class UserManagerHelper
    {
        public static async Task<AppUser> FindByEmailFromClaimsPrincipal(this UserManager<AppUser> userManager,
            ClaimsPrincipal user)
        {
            return await userManager.Users
                .SingleOrDefaultAsync(x => x.Email == user.FindFirstValue(ClaimTypes.Email));
        }

        //public static async Task<AppUser> FindByUserNameFromClaimsPrincipal(this UserManager<AppUser> userManager,
        //    ClaimsPrincipal user)
        //{
        //    return await userManager.Users
        //        .SingleOrDefaultAsync(x => x.UserName == user.FindFirstValue(ClaimTypes.Name));
        //}
    }
}
