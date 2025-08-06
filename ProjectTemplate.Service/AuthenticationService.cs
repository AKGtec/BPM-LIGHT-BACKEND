using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProjectTemplate.Contracts;
using ProjectTemplate.Service.Contracts;
using ProjectTemplate.Shared.DataTransferObjects;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectTemplate.Service;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public AuthenticationService(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        ILoggerManager logger,
        IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<AuthResponseDto> RegisterUserAsync(UserRegistrationDto userRegistration)
    {
        try
        {
            var user = new IdentityUser
            {
                UserName = userRegistration.UserName,
                Email = userRegistration.Email,
                PhoneNumber = userRegistration.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, userRegistration.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new AuthResponseDto
                {
                    IsAuthSuccessful = false,
                    ErrorMessage = errors
                };
            }

            // Add user to default role if needed
            await _userManager.AddToRoleAsync(user, "User");

            _logger.LogInfo($"User {user.UserName} registered successfully.");

            return new AuthResponseDto
            {
                IsAuthSuccessful = true,
                User = await MapToUserDto(user)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error during user registration: {ex.Message}");
            return new AuthResponseDto
            {
                IsAuthSuccessful = false,
                ErrorMessage = "Registration failed. Please try again."
            };
        }
    }

    public async Task<AuthResponseDto> LoginUserAsync(UserLoginDto userLogin)
    {
        try
        {
            var user = await _userManager.FindByNameAsync(userLogin.UserName) ??
                       await _userManager.FindByEmailAsync(userLogin.UserName);

            if (user == null)
            {
                return new AuthResponseDto
                {
                    IsAuthSuccessful = false,
                    ErrorMessage = "Invalid username or password."
                };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, userLogin.Password, false);

            if (!result.Succeeded)
            {
                return new AuthResponseDto
                {
                    IsAuthSuccessful = false,
                    ErrorMessage = "Invalid username or password."
                };
            }

            var token = await GenerateJwtToken(user);
            var tokenExpiration = DateTime.UtcNow.AddHours(1); // Token expires in 1 hour

            _logger.LogInfo($"User {user.UserName} logged in successfully.");

            return new AuthResponseDto
            {
                IsAuthSuccessful = true,
                Token = token,
                TokenExpiration = tokenExpiration,
                User = await MapToUserDto(user)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error during user login: {ex.Message}");
            return new AuthResponseDto
            {
                IsAuthSuccessful = false,
                ErrorMessage = "Login failed. Please try again."
            };
        }
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string token)
    {
        // For simplicity, we'll just validate the existing token and issue a new one
        // In a production environment, you'd want to implement proper refresh token logic
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["JWT:ValidIssuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["JWT:ValidAudience"],
                ValidateLifetime = false, // Don't validate lifetime for refresh
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return new AuthResponseDto
                {
                    IsAuthSuccessful = false,
                    ErrorMessage = "Invalid token."
                };
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    IsAuthSuccessful = false,
                    ErrorMessage = "User not found."
                };
            }

            var newToken = await GenerateJwtToken(user);
            var tokenExpiration = DateTime.UtcNow.AddHours(1);

            return new AuthResponseDto
            {
                IsAuthSuccessful = true,
                Token = newToken,
                TokenExpiration = tokenExpiration,
                User = await MapToUserDto(user)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error during token refresh: {ex.Message}");
            return new AuthResponseDto
            {
                IsAuthSuccessful = false,
                ErrorMessage = "Token refresh failed."
            };
        }
    }

    public async Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }

        return await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
    }

    public async Task<IdentityResult> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
    {
        var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
        if (user == null)
        {
            // Don't reveal that the user doesn't exist
            return IdentityResult.Success;
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        // In a real application, you would send this token via email
        _logger.LogInfo($"Password reset token generated for user {user.Email}: {token}");

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }

        return await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
    }

    public async Task<UserDto?> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user == null ? null : await MapToUserDto(user);
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user == null ? null : await MapToUserDto(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = _userManager.Users.ToList();
        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            userDtos.Add(await MapToUserDto(user));
        }

        return userDtos;
    }

    public async Task<IdentityResult> UpdateUserAsync(string userId, UserDto userDto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }

        user.UserName = userDto.UserName;
        user.Email = userDto.Email;
        user.PhoneNumber = userDto.PhoneNumber;

        return await _userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }

        return await _userManager.DeleteAsync(user);
    }

    public async Task<IdentityResult> AddUserToRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }

        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            await _roleManager.CreateAsync(new IdentityRole(roleName));
        }

        return await _userManager.AddToRoleAsync(user, roleName);
    }

    public async Task<IdentityResult> RemoveUserFromRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }

        return await _userManager.RemoveFromRoleAsync(user, roleName);
    }

    public async Task<IList<string>> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new List<string>();
        }

        return await _userManager.GetRolesAsync(user);
    }

    private async Task<string> GenerateJwtToken(IdentityUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!);
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _configuration["JWT:ValidIssuer"],
            Audience = _configuration["JWT:ValidAudience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private async Task<UserDto> MapToUserDto(IdentityUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName!,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber,
            Roles = roles
        };
    }
}
