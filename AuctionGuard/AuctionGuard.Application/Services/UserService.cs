using AuctionGuard.Application.DTOs.UserManagmentDTOs;
using AuctionGuard.Application.IServices;
using AuctionGuard.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using AuctionGuard.Domain.Interfaces;
using System.Web;

namespace AuctionGuard.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IUnitOfWork _unitOfWork; // <-- Inject IUnitOfWork
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;

        public UserService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager,
            IUnitOfWork unitOfWork, // <-- Add to constructor
            IConfiguration configuration,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork; // <-- Initialize
            _configuration = configuration;
            _emailSender = emailSender;
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            var permissions = new List<string>();

            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    permissions.AddRange(roleClaims.Where(c => c.Type == "Permission").Select(c => c.Value));
                }
            }

            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RegisterDate = user.RegisterDate,
                Roles = roles,
                Permissions = permissions.Distinct()
            };
        }
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var permissions = new List<string>();
                foreach (var roleName in roles)
                {
                    var role = await _roleManager.FindByNameAsync(roleName);
                    if (role != null)
                    {
                        var roleClaims = await _roleManager.GetClaimsAsync(role);
                        permissions.AddRange(roleClaims.Where(c => c.Type == "Permission").Select(c => c.Value));
                    }
                }

                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    RegisterDate = user.RegisterDate,
                    Roles = roles,
                    Permissions = permissions.Distinct()
                });
            }
            return userDtos;
        }

        public async Task<AuthenticationResult> RegisterAsync(RegisterDto registerDto)
        {
            var user = new User
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                FirstName = registerDto.FirstName,
                MiddleName = registerDto.MiddleName,
                LastName = registerDto.LastName,
                IdentificationImageUrl = registerDto.IdentificationImageUrl,
                RegisterDate = DateTime.UtcNow
            };
            var roleExist = await _roleManager.RoleExistsAsync(registerDto.Role);
            if (!roleExist)
            {
                throw new Exception("Role does not exist");
            }
            // UserManager handles the business logic and entity state, but doesn't save.
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return new AuthenticationResult { Succeeded = false, Errors = result.Errors.Select(e => e.Description) };
            }


            //temporary
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _userManager.ConfirmEmailAsync(user, token);

            await _userManager.AddToRoleAsync(user, registerDto.Role);

            // Use the UnitOfWork to commit the changes to the database.
            await _unitOfWork.CommitAsync();

            return await GenerateAuthenticationResultForUserAsync(user);
        }

        public async Task<AuthenticationResult> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Login)
                       ?? await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == loginDto.Login);

            if (user == null)
            {
                return new AuthenticationResult { Succeeded = false, Errors = new[] { "Invalid credentials." } };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return new AuthenticationResult { Succeeded = false, Errors = new[] { "Invalid credentials." } };
            }
            var res = await _signInManager.PasswordSignInAsync(user,loginDto.Password, loginDto.RememberMe, false);
            return await GenerateAuthenticationResultForUserAsync(user);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            var permissions = new List<string>();

            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    permissions.AddRange(roleClaims.Where(c => c.Type == "Permission").Select(c => c.Value));
                }
            }

            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RegisterDate = user.RegisterDate,
                Roles = roles,
                Permissions = permissions.Distinct()
            };
        }

        public async Task<AuthenticationResult> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return new AuthenticationResult { Succeeded = false, Errors = new[] { "User not found." } };
            }

            // Update properties from the DTO
            user.FirstName = updateUserDto.FirstName ?? user.FirstName;
            user.MiddleName = updateUserDto.MiddleName ?? user.MiddleName;
            user.LastName = updateUserDto.LastName ?? user.LastName;
            user.PhoneNumber = updateUserDto.PhoneNumber ?? user.PhoneNumber;
            user.Address = updateUserDto.Address ?? user.Address;

            // UserManager handles its own saving.
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                // Return the specific Identity errors
                return new AuthenticationResult { Succeeded = false, Errors = result.Errors.Select(e => e.Description) };
            }

            return new AuthenticationResult { Succeeded = true };
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
             if(!result.Succeeded)
            {
                throw new Exception(message: "aldaaer was not deleted");
            }
            return true; 
            
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist.
                return true;
            }

            // Generate the token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // URL-encode the token to make it safe for a URL
            var encodedToken = HttpUtility.UrlEncode(token);

            // Construct the password reset link
            // In a real app, the base URL would come from configuration
            var resetLink = $"https://your-frontend-app.com/reset-password?email={user.Email}&token={encodedToken}";

            // Construct the email message
            var subject = "Reset Your Password";
            var message = $"<p>Please reset your password by <a href='{resetLink}'>clicking here</a>.</p>" +
                          $"<p>If you cannot click the link, copy and paste this into your browser:</p>" +
                          $"<p>{resetLink}</p>" +
                          $"<p>For your security, this token is:</p><p><code>{token}</code></p>";

            // Use the email sender to send the email
            await _emailSender.SendEmailAsync(user.Email, subject, message);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null) return false;

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);

            if (result.Succeeded)
            {
                // Commit the password change
                await _unitOfWork.CommitAsync();
            }

            return result.Succeeded;
        }

        // The GenerateAuthenticationResultForUserAsync method remains the same as it does not modify data.
        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(User user)
        {
            // ... (Implementation from previous response)
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("id", user.Id.ToString())
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (var roleClaim in roleClaims)
                    {
                        // Assuming permissions are stored as claims on the role
                        if (roleClaim.Type == "Permission")
                        {
                            claims.Add(new Claim("Permission", roleClaim.Value));
                        }
                    }
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(TimeSpan.Parse(_configuration["Jwt:Expires"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthenticationResult
            {
                Succeeded = true,
                Token = tokenHandler.WriteToken(token)
            };
        }
    }
}
