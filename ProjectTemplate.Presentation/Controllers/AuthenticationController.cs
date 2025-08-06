using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTemplate.Service.Contracts;
using ProjectTemplate.Shared.DataTransferObjects;
using System.Security.Claims;

namespace ProjectTemplate.Presentation.Controllers;

public class AuthenticationController : BaseApiController
{
    public AuthenticationController(IServiceManager serviceManager) : base(serviceManager)
    {
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationDto userRegistration)
    {
        try
        {
            if (userRegistration == null)
                return BadRequest("User registration data is null.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _serviceManager.AuthenticationService.RegisterUserAsync(userRegistration);

            if (!result.IsAuthSuccessful)
                return BadRequest(result.ErrorMessage);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Login user
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto userLogin)
    {
        try
        {
            if (userLogin == null)
                return BadRequest("User login data is null.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _serviceManager.AuthenticationService.LoginUserAsync(userLogin);

            if (!result.IsAuthSuccessful)
                return Unauthorized(result.ErrorMessage);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Refresh JWT token
    /// </summary>
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("Token is required.");

            var result = await _serviceManager.AuthenticationService.RefreshTokenAsync(token);

            if (!result.IsAuthSuccessful)
                return Unauthorized(result.ErrorMessage);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Change user password
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        try
        {
            if (changePasswordDto == null)
                return BadRequest("Change password data is null.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var result = await _serviceManager.AuthenticationService.ChangePasswordAsync(userId, changePasswordDto);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(errors);
            }

            return Ok("Password changed successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Forgot password
    /// </summary>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        try
        {
            if (forgotPasswordDto == null)
                return BadRequest("Forgot password data is null.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _serviceManager.AuthenticationService.ForgotPasswordAsync(forgotPasswordDto);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(errors);
            }

            return Ok("Password reset instructions have been sent to your email.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Reset password
    /// </summary>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        try
        {
            if (resetPasswordDto == null)
                return BadRequest("Reset password data is null.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _serviceManager.AuthenticationService.ResetPasswordAsync(resetPasswordDto);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(errors);
            }

            return Ok("Password reset successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var user = await _serviceManager.AuthenticationService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get all users (Admin only)
    /// </summary>
    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var users = await _serviceManager.AuthenticationService.GetAllUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get user by ID (Admin only)
    /// </summary>
    [HttpGet("users/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserById(string id)
    {
        try
        {
            var user = await _serviceManager.AuthenticationService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound("User not found.");

            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Add user to role (Admin only)
    /// </summary>
    [HttpPost("users/{userId}/roles/{roleName}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddUserToRole(string userId, string roleName)
    {
        try
        {
            var result = await _serviceManager.AuthenticationService.AddUserToRoleAsync(userId, roleName);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(errors);
            }

            return Ok($"User added to role {roleName} successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Remove user from role (Admin only)
    /// </summary>
    [HttpDelete("users/{userId}/roles/{roleName}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveUserFromRole(string userId, string roleName)
    {
        try
        {
            var result = await _serviceManager.AuthenticationService.RemoveUserFromRoleAsync(userId, roleName);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(errors);
            }

            return Ok($"User removed from role {roleName} successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get user roles
    /// </summary>
    [HttpGet("users/{userId}/roles")]
    [Authorize]
    public async Task<IActionResult> GetUserRoles(string userId)
    {
        try
        {
            // Users can only get their own roles unless they're admin
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");

            if (currentUserId != userId && !isAdmin)
                return Forbid("You can only access your own roles.");

            var roles = await _serviceManager.AuthenticationService.GetUserRolesAsync(userId);
            return Ok(roles);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
