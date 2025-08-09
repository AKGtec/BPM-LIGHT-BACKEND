using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTemplate.Models.Entities;
using ProjectTemplate.Service.Contracts;
using ProjectTemplate.Shared.DataTransferObjects;
using System.Security.Claims;

namespace ProjectTemplate.Presentation.Controllers;

public class PerformanceReviewController : BaseApiController
{
    public PerformanceReviewController(IServiceManager serviceManager) : base(serviceManager)
    {
    }

    /// <summary>
    /// Get all performance reviews with pagination and filtering
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetPerformanceReviews([FromQuery] PerformanceReviewParameters? parameters)
    {
        try
        {
            parameters ??= new PerformanceReviewParameters();
            var performanceReviews = await _serviceManager.PerformanceReviewService.GetPerformanceReviewsAsync(parameters, trackChanges: false);
            return Ok(performanceReviews);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get current user's performance reviews
    /// </summary>
    [HttpGet("my-reviews")]
    public async Task<IActionResult> GetMyPerformanceReviews([FromQuery] PerformanceReviewParameters? parameters)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            parameters ??= new PerformanceReviewParameters();
            var performanceReviews = await _serviceManager.PerformanceReviewService.GetMyPerformanceReviewsAsync(userId, parameters, trackChanges: false);
            return Ok(performanceReviews);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get pending performance reviews
    /// </summary>
    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingPerformanceReviews([FromQuery] PerformanceReviewParameters? parameters)
    {
        try
        {
            parameters ??= new PerformanceReviewParameters();
            var performanceReviews = await _serviceManager.PerformanceReviewService.GetPendingPerformanceReviewsAsync(parameters, trackChanges: false);
            return Ok(performanceReviews);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get performance review by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPerformanceReview(Guid id)
    {
        try
        {
            var performanceReview = await _serviceManager.PerformanceReviewService.GetPerformanceReviewByIdAsync(id, trackChanges: false);
            if (performanceReview == null)
                return NotFound($"Performance review with ID {id} not found.");

            return Ok(performanceReview);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Create a new performance review
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreatePerformanceReview([FromBody] CreatePerformanceReviewDto reviewDto)
    {
        try
        {
            if (reviewDto == null)
                return BadRequest("Performance review data is null.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var performanceReview = await _serviceManager.PerformanceReviewService.CreatePerformanceReviewAsync(reviewDto, userId);
            return CreatedAtAction(nameof(GetPerformanceReview), new { id = performanceReview.Id }, performanceReview);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Update an existing performance review
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdatePerformanceReview(Guid id, [FromBody] UpdatePerformanceReviewDto reviewDto)
    {
        try
        {
            if (reviewDto == null)
                return BadRequest("Performance review data is null.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingReview = await _serviceManager.PerformanceReviewService.GetPerformanceReviewByIdAsync(id, trackChanges: false);
            if (existingReview == null)
                return NotFound($"Performance review with ID {id} not found.");

            var updatedReview = await _serviceManager.PerformanceReviewService.UpdatePerformanceReviewAsync(id, reviewDto, trackChanges: true);
            return Ok(updatedReview);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Delete a performance review
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletePerformanceReview(Guid id)
    {
        try
        {
            var performanceReview = await _serviceManager.PerformanceReviewService.GetPerformanceReviewByIdAsync(id, trackChanges: false);
            if (performanceReview == null)
                return NotFound($"Performance review with ID {id} not found.");

            await _serviceManager.PerformanceReviewService.DeletePerformanceReviewAsync(id, trackChanges: false);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Start a performance review
    /// </summary>
    [HttpPost("{id:guid}/start")]
    public async Task<IActionResult> StartReview(Guid id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            await _serviceManager.PerformanceReviewService.StartReviewAsync(id, userId);
            return Ok(new { message = "Performance review started successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Complete a performance review
    /// </summary>
    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> CompleteReview(Guid id, [FromBody] CompleteReviewDto completeDto)
    {
        try
        {
            if (completeDto == null)
                return BadRequest("Complete review data is null.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            await _serviceManager.PerformanceReviewService.CompleteReviewAsync(id, completeDto, userId);
            return Ok(new { message = "Performance review completed successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Cancel a performance review
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> CancelReview(Guid id, [FromBody] CancelReviewDto? cancelDto = null)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            cancelDto ??= new CancelReviewDto();
            await _serviceManager.PerformanceReviewService.CancelReviewAsync(id, cancelDto, userId);
            return Ok(new { message = "Performance review cancelled successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Bulk update status of performance reviews
    /// </summary>
    [HttpPost("bulk-update-status")]
    public async Task<IActionResult> BulkUpdateStatus([FromBody] BulkUpdateStatusDto bulkUpdateDto)
    {
        try
        {
            if (bulkUpdateDto?.Ids == null || !bulkUpdateDto.Ids.Any())
                return BadRequest("No performance review IDs provided.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            await _serviceManager.PerformanceReviewService.BulkUpdateStatusAsync(bulkUpdateDto, userId);
            return Ok(new { message = $"{bulkUpdateDto.Ids.Count} performance reviews updated successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Bulk delete performance reviews
    /// </summary>
    [HttpPost("bulk-delete")]
    public async Task<IActionResult> BulkDelete([FromBody] BulkDeleteDto bulkDeleteDto)
    {
        try
        {
            if (bulkDeleteDto?.Ids == null || !bulkDeleteDto.Ids.Any())
                return BadRequest("No performance review IDs provided.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            await _serviceManager.PerformanceReviewService.BulkDeleteAsync(bulkDeleteDto, userId);
            return Ok(new { message = $"{bulkDeleteDto.Ids.Count} performance reviews deleted successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get performance review statistics
    /// </summary>
    [HttpGet("statistics")]
    public async Task<IActionResult> GetPerformanceReviewStatistics([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var statistics = await _serviceManager.PerformanceReviewService.GetPerformanceReviewStatisticsAsync(startDate, endDate);
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Export performance reviews report
    /// </summary>
    [HttpGet("export")]
    public async Task<IActionResult> ExportPerformanceReviews([FromQuery] string format = "excel", [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        try
        {
            if (format != "excel" && format != "pdf")
                return BadRequest("Format must be 'excel' or 'pdf'.");

            var reportData = await _serviceManager.PerformanceReviewService.ExportPerformanceReviewsAsync(format, startDate, endDate);
            
            var contentType = format == "excel" ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "application/pdf";
            var fileName = $"performance-reviews-{DateTime.Now:yyyyMMdd}.{(format == "excel" ? "xlsx" : "pdf")}";
            
            return File(reportData, contentType, fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get review type label
    /// </summary>
    [HttpGet("types/{type}/label")]
    public IActionResult GetReviewTypeLabel(ReviewType type)
    {
        try
        {
            var label = _serviceManager.PerformanceReviewService.GetReviewTypeLabel(type);
            return Ok(new { type, label });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get review status label
    /// </summary>
    [HttpGet("status/{status}/label")]
    public IActionResult GetReviewStatusLabel(ReviewStatus status)
    {
        try
        {
            var label = _serviceManager.PerformanceReviewService.GetReviewStatusLabel(status);
            return Ok(new { status, label });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get review priority label
    /// </summary>
    [HttpGet("priority/{priority}/label")]
    public IActionResult GetReviewPriorityLabel(ReviewPriority priority)
    {
        try
        {
            var label = _serviceManager.PerformanceReviewService.GetReviewPriorityLabel(priority);
            return Ok(new { priority, label });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get review type options for dropdowns
    /// </summary>
    [HttpGet("types/options")]
    public IActionResult GetReviewTypeOptions()
    {
        try
        {
            var options = _serviceManager.PerformanceReviewService.GetReviewTypeOptions();
            return Ok(options);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get review status options for dropdowns
    /// </summary>
    [HttpGet("status/options")]
    public IActionResult GetReviewStatusOptions()
    {
        try
        {
            var options = _serviceManager.PerformanceReviewService.GetReviewStatusOptions();
            return Ok(options);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get review priority options for dropdowns
    /// </summary>
    [HttpGet("priority/options")]
    public IActionResult GetReviewPriorityOptions()
    {
        try
        {
            var options = _serviceManager.PerformanceReviewService.GetReviewPriorityOptions();
            return Ok(options);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}