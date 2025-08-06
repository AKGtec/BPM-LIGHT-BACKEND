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
    public async Task<IActionResult> GetRequests([FromQuery] RequestParameters parameters)
    {
        try
        {
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
    public async Task<IActionResult> GetMyRequests([FromQuery] RequestParameters parameters)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

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
    public async Task<IActionResult> GetRequestsByStatus(RequestStatus status, [FromQuery] RequestParameters parameters)
    {
        try
        {
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
    public async Task<IActionResult> GetPendingApprovals([FromQuery] RequestParameters parameters)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

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
}
