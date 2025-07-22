using AuctionGuard.Application.DTOs.UserManagmentDTOs;
using AuctionGuard.Application.IServices;
using AuctionGuard.Application.Authorization;
using AuctionGuard.Infrastructure.Seeders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace AuctionGuard.API.Controllers.UserController
{
    /// <summary>
    /// API controller for managing users.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication for all actions by default
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Gets a list of all users. Requires Admin role and View Users permission.
        /// </summary>
        [HttpGet]
        [HasPermission(Permissions.Users.View)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPost("email")]
        [HasPermission(Permissions.Users.View)]
        public async Task<IActionResult> GetUserByEmail([FromBody]string Login)
        {
            var userEmail = await _userService.GetUserByEmailAsync(Login);
            return Ok(userEmail);
        }

        /// <summary>
        /// Registers a new user. This endpoint is open to the public.
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.RegisterAsync(registerDto);

            if (!result.Succeeded)
            {
                return BadRequest(new { Errors = result.Errors });
            }

            return Ok(new { Token = result.Token });
        }

        /// <summary>
        /// Authenticates a user and provides a JWT. This endpoint is open to the public.
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // Check if the user is already authenticated in the current context
            if (User.Identity.IsAuthenticated)
            {
                return BadRequest(new { Message = "User is already logged in." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.LoginAsync(loginDto);

            if (!result.Succeeded)
            {
                return Unauthorized(new { Errors = result.Errors });
            }
            var user = await _userService.GetUserByEmailAsync(loginDto.Login);
            
            return Ok(new { Token = result.Token ,user});
        }

        

        /// <summary>
        /// Logs out the user. Accessible by any authenticated user.
        /// </summary>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _userService.LogoutAsync();
            return Ok(new { Message = "Logged out successfully." });
        }

        /// <summary>
        /// Gets a user's profile by their ID. Accessible by the user themselves or an admin.
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id.ToString() != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        /// <summary>
        /// Updates a user's profile. Accessible by the user themselves or an admin. Requires Edit permission.
        /// </summary>
        [HttpPut("{id:guid}")]
        [HasPermission(Permissions.Users.Edit)]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id.ToString() != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var result = await _userService.UpdateUserAsync(id, updateUserDto);

            if (!result.Succeeded)
            {
                return BadRequest(new { Errors = result.Errors });
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a user account. Accessible by the user themselves or an admin. Requires Delete permission.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [HasPermission(Permissions.Users.Delete)]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id.ToString() != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var result = await _userService.DeleteUserAsync(id);

            if (!result)
            {
                return NotFound(new { Message = "User not found or deletion failed." });
            }

            return NoContent();
        }

        /// <summary>
        /// Sends a password reset token. This endpoint is open to the public.
        /// </summary>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _userService.ForgotPasswordAsync(forgotPasswordDto);
            return Ok(new { Message = "If an account with that email exists, a password reset link has been sent." });
        }

        /// <summary>
        /// Resets the user's password using a token. This endpoint is open to the public.
        /// </summary>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.ResetPasswordAsync(resetPasswordDto);

            if (!result)
            {
                return BadRequest(new { Message = "Failed to reset password. The token may be invalid or expired." });
            }

            return Ok(new { Message = "Password has been reset successfully." });
        }
    }
}
