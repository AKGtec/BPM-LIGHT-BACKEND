using Microsoft.AspNetCore.Identity;
using ProjectTemplate.Shared.DataTransferObjects;

namespace ProjectTemplate.Service.Contracts;

public interface IAuthenticationService
{
    Task<AuthResponseDto> RegisterUserAsync(UserRegistrationDto userRegistration);
    Task<AuthResponseDto> LoginUserAsync(UserLoginDto userLogin);
    Task<AuthResponseDto> RefreshTokenAsync(string token);
    Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);
    Task<IdentityResult> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
    Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    Task<UserDto?> GetUserByIdAsync(string userId);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<IdentityResult> UpdateUserAsync(string userId, UserDto userDto);
    Task<IdentityResult> DeleteUserAsync(string userId);
    Task<IdentityResult> AddUserToRoleAsync(string userId, string roleName);
    Task<IdentityResult> RemoveUserFromRoleAsync(string userId, string roleName);
    Task<IList<string>> GetUserRolesAsync(string userId);
}
