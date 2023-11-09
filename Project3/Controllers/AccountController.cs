using BiometricAttendanceSystem.DTO;
using BiometricAttendanceSystem.Errors;
using BiometricAttendanceSystem.Helper;
using Core.Entities.Identity;
using Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BiometricAttendanceSystem.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user == null)
            {
                return Unauthorized(new ApiResponse(401));
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new ApiResponse(401));
            }

            return new UserDto
            {
                Token =_tokenService.CreateToken(user),
                Username = user.UserName,
                Email = user.Email
            };
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userManager.FindByEmailFromClaimsPrincipal(User);

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
             
            };
        }

        [HttpGet("Email Exists")]
        public async Task<ActionResult<bool>> CheckEmailExistsAsync([FromQuery] string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }

        [Authorize]
        [HttpPost("[action]")]
        public async Task<ActionResult<UserDto>> EditProfile(EditProfileDto editProfileDto)
        {
            var user = await _userManager.FindByEmailFromClaimsPrincipal(User);
            if(user == null)
            {
                return NotFound(new ApiResponse(404, "User Not Found"));
            }

            user.UserName = editProfileDto.Username;
            user.Email = editProfileDto.Email;

            var result = await _userManager.UpdateAsync(user);

            if(!result.Succeeded)
            {
                return BadRequest(new ApiResponse(400, "Failed to Update Profile"));
            }

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                Email = user.Email
            };
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<ActionResult<UserDto>> ChangePassword(ChangePassDto changePassDto)
        {
            var user = await _userManager.FindByEmailFromClaimsPrincipal(User);

            if (user == null)
            {
                return NotFound(new ApiResponse(404, "User not found"));
            }
            if (!await _userManager.CheckPasswordAsync(user, changePassDto.currentPassword))
            {
                return BadRequest(new ApiResponse(400, "Wrong current password"));
            }

            var passwordValidator = _userManager.PasswordValidators.FirstOrDefault();
            var result = await passwordValidator.ValidateAsync(_userManager, user, changePassDto.newPassword);

            if (!result.Succeeded)
            {

                return BadRequest(new ApiResponse(400, "Password must contain at least one non-alphanumeric character (e.g., special symbol like !, @, #) and a capital letter"));
            }


            var changeResult = await _userManager.ChangePasswordAsync(user, changePassDto.currentPassword, changePassDto.newPassword);

            if (!changeResult.Succeeded)
            {

                return BadRequest(new ApiResponse(400, "Failed to change password"));
            }

            return Ok(new ApiResponse(200, "Password changed successfully"));
        }



    }
}
