using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTemplate.Models.Entities;
using ProjectTemplate.Service.Contracts;
using ProjectTemplate.Shared.DataTransferObjects;
using ProjectTemplate.Shared.RequestFeatures;
using System.Security.Claims;

namespace ProjectTemplate.Presentation.Controllers;

public class RequestController : BaseApiController
{
    public RequestController(IServiceManager serviceManager) : base(serviceManager)
    {
    }

    /// <summary>
    /// Get all requests with pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetRequests([FromQuery] EntityParameters? parameters)
    {
        try
        {
            parameters ??= new EntityParameters();
            var requests = await _serviceManager.RequestService.GetAllAsync(parameters, trackChanges: false);
            return Ok(requests);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get request by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetRequest(Guid id)
    {
        try
        {
            var request = await _serviceManager.RequestService.GetByIdAsync(id, trackChanges: false);
            if (request == null)
                return NotFound($"Request with ID {id} not found.");

            return Ok(request);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get request with its steps
    /// </summary>
    [HttpGet("{id:guid}/steps")]
    public async Task<IActionResult> GetRequestWithSteps(Guid id)
    {
        try
        {
            var request = await _serviceManager.RequestService.GetRequestWithStepsAsync(id, trackChanges: false);
            if (request == null)
                return NotFound($"Request with ID {id} not found.");

            return Ok(request);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get requests by current user
    /// </summary>
    [HttpGet("my-requests")]
    public async Task<IActionResult> GetMyRequests([FromQuery] EntityParameters? parameters)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            parameters ??= new EntityParameters();
            var requests = await _serviceManager.RequestService.GetRequestsByUserAsync(userId, parameters, trackChanges: false);
            return Ok(requests);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get requests by status
    /// </summary>
    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetRequestsByStatus(RequestStatus status, [FromQuery] EntityParameters? parameters)
    {
        try
        {
            parameters ??= new EntityParameters();
            var requests = await _serviceManager.RequestService.GetRequestsByStatusAsync(status, parameters, trackChanges: false);
            return Ok(requests);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get pending requests for current user to approve
    /// </summary>
    [HttpGet("pending-approvals")]
    public async Task<IActionResult> GetPendingApprovals([FromQuery] EntityParameters? parameters)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            parameters ??= new EntityParameters();
            var requests = await _serviceManager.RequestService.GetPendingRequestsForUserAsync(userId, parameters, trackChanges: false);
            return Ok(requests);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Create a new request
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateRequest([FromBody] CreateRequestDto requestDto)
    {
        try
        {
            if (requestDto == null)
                return BadRequest("Request data is null.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var request = await _serviceManager.RequestService.CreateRequestAsync(requestDto, userId);
            return CreatedAtAction(nameof(GetRequest), new { id = request.Id }, request);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Update an existing request
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateRequest(Guid id, [FromBody] UpdateRequestDto requestDto)
    {
        try
        {
            if (requestDto == null)
                return BadRequest("Request data is null.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingRequest = await _serviceManager.RequestService.GetByIdAsync(id, trackChanges: false);
            if (existingRequest == null)
                return NotFound($"Request with ID {id} not found.");

            var mappedDto = new RequestDto
            {
                Id = id,
                Type = requestDto.Type,
                Description = requestDto.Description,
                Title = requestDto.Title,
                Status = requestDto.Status,
                InitiatorId = existingRequest.InitiatorId
            };

            await _serviceManager.RequestService.UpdateAsync(id, mappedDto, trackChanges: true);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Delete a request
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteRequest(Guid id)
    {
        try
        {
            var request = await _serviceManager.RequestService.GetByIdAsync(id, trackChanges: false);
            if (request == null)
                return NotFound($"Request with ID {id} not found.");

            await _serviceManager.RequestService.DeleteAsync(id, trackChanges: false);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Approve a request step
    /// </summary>
    [HttpPost("{requestId:guid}/steps/{stepId:guid}/approve")]
    public async Task<IActionResult> ApproveStep(Guid requestId, Guid stepId, [FromBody] ApproveRejectStepDto dto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var request = await _serviceManager.RequestService.ApproveRequestStepAsync(requestId, stepId, userId, dto?.Comments);
            return Ok(request);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Reject a request step
    /// </summary>
    [HttpPost("{requestId:guid}/steps/{stepId:guid}/reject")]
    public async Task<IActionResult> RejectStep(Guid requestId, Guid stepId, [FromBody] ApproveRejectStepDto dto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var request = await _serviceManager.RequestService.RejectRequestStepAsync(requestId, stepId, userId, dto?.Comments);
            return Ok(request);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get request summary statistics
    /// </summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetRequestSummary()
    {
        try
        {
            var summary = await _serviceManager.RequestService.GetRequestSummaryAsync();
            return Ok(summary);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get requests by type
    /// </summary>
    [HttpGet("by-type")]
    public async Task<IActionResult> GetRequestsByType([FromQuery] RequestType type, [FromQuery] EntityParameters? parameters)
    {
        try
        {
            parameters ??= new EntityParameters();
            var requests = await _serviceManager.RequestService.GetRequestsByTypeAsync(type, parameters, trackChanges: false);
            return Ok(requests);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get requests for a specific role
    /// </summary>
    [HttpGet("for-role")]
    public async Task<IActionResult> GetRequestsForRole([FromQuery] string role, [FromQuery] EntityParameters? parameters)
    {
        try
        {
            if (string.IsNullOrEmpty(role))
                return BadRequest("Role parameter is required.");

            parameters ??= new EntityParameters();
            var requests = await _serviceManager.RequestService.GetRequestsForRoleAsync(role, parameters, trackChanges: false);
            return Ok(requests);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get requests by user ID
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetRequestsByUser(string userId, [FromQuery] EntityParameters? parameters)
    {
        try
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID is required.");

            parameters ??= new EntityParameters();
            var requests = await _serviceManager.RequestService.GetRequestsByUserAsync(userId, parameters, trackChanges: false);
            return Ok(requests);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get requests by date range
    /// </summary>
    [HttpGet("date-range")]
    public async Task<IActionResult> GetRequestsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] EntityParameters? parameters)
    {
        try
        {
            if (startDate == default || endDate == default)
                return BadRequest("Start date and end date are required.");

            if (startDate > endDate)
                return BadRequest("Start date cannot be greater than end date.");

            parameters ??= new EntityParameters();
            var requests = await _serviceManager.RequestService.GetRequestsByDateRangeAsync(startDate, endDate, parameters, trackChanges: false);
            return Ok(requests);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get requests by multiple filters
    /// </summary>
    [HttpPost("filter")]
    public async Task<IActionResult> GetRequestsByFilter([FromBody] RequestFilterDto filter, [FromQuery] EntityParameters? parameters)
    {
        try
        {
            if (filter == null)
                return BadRequest("Filter data is required.");

            parameters ??= new EntityParameters();
            var requests = await _serviceManager.RequestService.GetRequestsByFilterAsync(filter, parameters, trackChanges: false);
            return Ok(requests);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Cancel a request
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> CancelRequest(Guid id, [FromBody] CancelRequestDto dto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var request = await _serviceManager.RequestService.CancelRequestAsync(id, userId, dto?.Reason);
            return Ok(request);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Archive a request
    /// </summary>
    [HttpPost("{id:guid}/archive")]
    public async Task<IActionResult> ArchiveRequest(Guid id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            await _serviceManager.RequestService.ArchiveRequestAsync(id, userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Restore an archived request
    /// </summary>
    [HttpPost("{id:guid}/restore")]
    public async Task<IActionResult> RestoreRequest(Guid id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            await _serviceManager.RequestService.RestoreRequestAsync(id, userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get request history/audit trail
    /// </summary>
    [HttpGet("{id:guid}/history")]
    public async Task<IActionResult> GetRequestHistory(Guid id)
    {
        try
        {
            var history = await _serviceManager.RequestService.GetRequestHistoryAsync(id);
            return Ok(history);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Bulk approve requests
    /// </summary>
    [HttpPost("bulk-approve")]
    public async Task<IActionResult> BulkApproveRequests([FromBody] BulkActionDto dto)
    {
        try
        {
            if (dto == null || !dto.RequestIds.Any())
                return BadRequest("Request IDs are required.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var result = await _serviceManager.RequestService.BulkApproveRequestsAsync(dto.RequestIds, userId, dto.Comments);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Bulk reject requests
    /// </summary>
    [HttpPost("bulk-reject")]
    public async Task<IActionResult> BulkRejectRequests([FromBody] BulkActionDto dto)
    {
        try
        {
            if (dto == null || !dto.RequestIds.Any())
                return BadRequest("Request IDs are required.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var result = await _serviceManager.RequestService.BulkRejectRequestsAsync(dto.RequestIds, userId, dto.Comments);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get requests statistics
    /// </summary>
    [HttpGet("statistics")]
    public async Task<IActionResult> GetRequestStatistics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        try
        {
            var statistics = await _serviceManager.RequestService.GetRequestStatisticsAsync(startDate, endDate);
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
