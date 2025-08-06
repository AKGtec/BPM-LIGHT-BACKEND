using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTemplate.Service.Contracts;
using ProjectTemplate.Shared.DataTransferObjects;
using ProjectTemplate.Shared.RequestFeatures;
using System.Security.Claims;

namespace ProjectTemplate.Presentation.Controllers;

public class NotificationController : BaseApiController
{
    public NotificationController(IServiceManager serviceManager) : base(serviceManager)
    {
    }

    /// <summary>
    /// Get all notifications with pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetNotifications([FromQuery] RequestParameters parameters)
    {
        try
        {
            var notifications = await _serviceManager.NotificationService.GetAllAsync(parameters, trackChanges: false);
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get notification by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetNotification(Guid id)
    {
        try
        {
            var notification = await _serviceManager.NotificationService.GetByIdAsync(id, trackChanges: false);
            if (notification == null)
                return NotFound($"Notification with ID {id} not found.");

            return Ok(notification);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get notifications for current user
    /// </summary>
    [HttpGet("my-notifications")]
    public async Task<IActionResult> GetMyNotifications([FromQuery] RequestParameters parameters)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var notifications = await _serviceManager.NotificationService.GetUserNotificationsAsync(userId, parameters, trackChanges: false);
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get unread notifications for current user
    /// </summary>
    [HttpGet("unread")]
    public async Task<IActionResult> GetUnreadNotifications([FromQuery] RequestParameters parameters)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var notifications = await _serviceManager.NotificationService.GetUnreadNotificationsAsync(userId, parameters, trackChanges: false);
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get unread notification count for current user
    /// </summary>
    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var count = await _serviceManager.NotificationService.GetUnreadCountAsync(userId, trackChanges: false);
            return Ok(new { count });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Create a new notification
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationDto notificationDto)
    {
        try
        {
            if (notificationDto == null)
                return BadRequest("Notification data is null.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var notification = await _serviceManager.NotificationService.CreateNotificationAsync(notificationDto);
            return CreatedAtAction(nameof(GetNotification), new { id = notification.Id }, notification);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Update an existing notification
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateNotification(Guid id, [FromBody] UpdateNotificationDto notificationDto)
    {
        try
        {
            if (notificationDto == null)
                return BadRequest("Notification data is null.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingNotification = await _serviceManager.NotificationService.GetByIdAsync(id, trackChanges: false);
            if (existingNotification == null)
                return NotFound($"Notification with ID {id} not found.");

            var mappedDto = new NotificationDto
            {
                Id = id,
                UserId = existingNotification.UserId,
                Message = notificationDto.Message,
                IsRead = notificationDto.IsRead,
                Type = notificationDto.Type,
                ActionUrl = notificationDto.ActionUrl
            };

            await _serviceManager.NotificationService.UpdateAsync(id, mappedDto, trackChanges: true);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Delete a notification
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteNotification(Guid id)
    {
        try
        {
            var notification = await _serviceManager.NotificationService.GetByIdAsync(id, trackChanges: false);
            if (notification == null)
                return NotFound($"Notification with ID {id} not found.");

            await _serviceManager.NotificationService.DeleteAsync(id, trackChanges: false);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Mark a notification as read
    /// </summary>
    [HttpPatch("{id:guid}/mark-read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        try
        {
            await _serviceManager.NotificationService.MarkAsReadAsync(id, trackChanges: true);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Mark all notifications as read for current user
    /// </summary>
    [HttpPatch("mark-all-read")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            await _serviceManager.NotificationService.MarkAllAsReadAsync(userId, trackChanges: true);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
