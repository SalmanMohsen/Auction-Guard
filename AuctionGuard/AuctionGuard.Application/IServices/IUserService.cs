using AuctionGuard.Application.DTOs.UserManagmentDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.IServices
{
    public interface IUserService
    {
        Task<AuthenticationResult> RegisterAsync(RegisterDto registerDto);
        Task<AuthenticationResult> LoginAsync(LoginDto loginDto);
        Task LogoutAsync();
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(Guid userId);
        Task<AuthenticationResult> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(Guid userId);
        Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    }
}
