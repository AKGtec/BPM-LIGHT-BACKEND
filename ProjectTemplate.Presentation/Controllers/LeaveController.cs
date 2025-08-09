using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTemplate.Models.Entities;
using ProjectTemplate.Presentation.Models;
using ProjectTemplate.Service.Contracts;
using ProjectTemplate.Shared.DataTransferObjects;
using System.Security.Claims;

namespace ProjectTemplate.Presentation.Controllers;

public class LeaveController : BaseApiController
{
    public LeaveController(IServiceManager serviceManager) : base(serviceManager)
    {
    }

    /// <summary>
    /// Get all leave requests with pagination and filtering
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetLeaveRequests([FromQuery] LeaveRequestParameters? parameters)
    {
        try
        {
            parameters ??= new LeaveRequestParameters();
            var leaveRequests = await _serviceManager.LeaveService.GetLeaveRequestsAsync(parameters, trackChanges: false);
            return Ok(leaveRequests);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get pending leave requests
    /// </summary>
    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingLeaveRequests([FromQuery] LeaveRequestParameters? parameters)
    {
        try
        {
            parameters ??= new LeaveRequestParameters();
            var leaveRequests = await _serviceManager.LeaveService.GetPendingLeaveRequestsAsync(parameters, trackChanges: false);
            return Ok(leaveRequests);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get current user's leave requests
    /// </summary>
    [HttpGet("my-requests")]
    public async Task<IActionResult> GetMyLeaveRequests([FromQuery] LeaveRequestParameters? parameters)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            parameters ??= new LeaveRequestParameters();
            var leaveRequests = await _serviceManager.LeaveService.GetMyLeaveRequestsAsync(userId, parameters, trackChanges: false);
            return Ok(leaveRequests);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get leave request by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetLeaveRequest(Guid id)
    {
        try
        {
            var leaveRequest = await _serviceManager.LeaveService.GetLeaveRequestByIdAsync(id, trackChanges: false);
            if (leaveRequest == null)
                return NotFound($"Leave request with ID {id} not found.");

            return Ok(leaveRequest);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Create a new leave request (JSON)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateLeaveRequest([FromBody] CreateLeaveRequestDto requestDto)
    {
        try
        {
            if (requestDto == null)
                return BadRequest("Leave request data is null.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var leaveRequest = await _serviceManager.LeaveService.CreateLeaveRequestAsync(requestDto, userId);
            return CreatedAtAction(nameof(GetLeaveRequest), new { id = leaveRequest.Id }, leaveRequest);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Create a new leave request with file uploads (Form Data)
    /// </summary>
    [HttpPost("with-files")]
    public async Task<IActionResult> CreateLeaveRequestWithFiles([FromForm] CreateLeaveRequestWithFilesDto requestDto)
    {
        try
        {
            if (requestDto == null)
                return BadRequest("Leave request data is null.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            // Convert to regular DTO
            var createDto = new CreateLeaveRequestDto
            {
                EmployeeId = requestDto.EmployeeId,
                LeaveType = requestDto.LeaveType,
                StartDate = requestDto.StartDate,
                EndDate = requestDto.EndDate,
                Reason = requestDto.Reason,
                AttachmentFileNames = requestDto.Attachments?.Select(f => f.FileName).ToList()
            };

            var leaveRequest = await _serviceManager.LeaveService.CreateLeaveRequestAsync(createDto, userId);
            return CreatedAtAction(nameof(GetLeaveRequest), new { id = leaveRequest.Id }, leaveRequest);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Approve a leave request
    /// </summary>
    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> ApproveLeaveRequest(Guid id, [FromBody] ApproveRejectLeaveDto data)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            await _serviceManager.LeaveService.ApproveLeaveRequestAsync(id, data, userId);
            return Ok(new { message = "Leave request approved successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Reject a leave request
    /// </summary>
    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> RejectLeaveRequest(Guid id, [FromBody] ApproveRejectLeaveDto data)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            await _serviceManager.LeaveService.RejectLeaveRequestAsync(id, data, userId);
            return Ok(new { message = "Leave request rejected successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Cancel a leave request
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> CancelLeaveRequest(Guid id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            await _serviceManager.LeaveService.CancelLeaveRequestAsync(id, userId);
            return Ok(new { message = "Leave request cancelled successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Bulk approve leave requests
    /// </summary>
    [HttpPost("bulk-approve")]
    public async Task<IActionResult> BulkApproveLeaveRequests([FromBody] BulkApproveRejectLeaveDto data)
    {
        try
        {
            if (data?.Ids == null || !data.Ids.Any())
                return BadRequest("No leave request IDs provided.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            await _serviceManager.LeaveService.BulkApproveLeaveRequestsAsync(data, userId);
            return Ok(new { message = $"{data.Ids.Count} leave requests approved successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Bulk reject leave requests
    /// </summary>
    [HttpPost("bulk-reject")]
    public async Task<IActionResult> BulkRejectLeaveRequests([FromBody] BulkApproveRejectLeaveDto data)
    {
        try
        {
            if (data?.Ids == null || !data.Ids.Any())
                return BadRequest("No leave request IDs provided.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            await _serviceManager.LeaveService.BulkRejectLeaveRequestsAsync(data, userId);
            return Ok(new { message = $"{data.Ids.Count} leave requests rejected successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get leave balances for all employees or specific employee
    /// </summary>
    [HttpGet("balances")]
    public async Task<IActionResult> GetLeaveBalances([FromQuery] string? employeeId = null)
    {
        try
        {
            var balances = await _serviceManager.LeaveService.GetLeaveBalancesAsync(employeeId);
            return Ok(balances);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get current user's leave balances
    /// </summary>
    [HttpGet("my-balances")]
    public async Task<IActionResult> GetMyLeaveBalances()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var balances = await _serviceManager.LeaveService.GetMyLeaveBalancesAsync(userId);
            return Ok(balances);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Update leave balance for an employee
    /// </summary>
    [HttpPost("balances/adjust")]
    public async Task<IActionResult> UpdateLeaveBalance([FromBody] UpdateLeaveBalanceDto updateDto)
    {
        try
        {
            if (updateDto == null)
                return BadRequest("Update data is null.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _serviceManager.LeaveService.UpdateLeaveBalanceAsync(updateDto);
            return Ok(new { message = "Leave balance updated successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get leave types configuration
    /// </summary>
    [HttpGet("types")]
    public async Task<IActionResult> GetLeaveTypes()
    {
        try
        {
            var leaveTypes = await _serviceManager.LeaveService.GetLeaveTypesAsync();
            return Ok(leaveTypes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get leave statistics
    /// </summary>
    [HttpGet("statistics")]
    public async Task<IActionResult> GetLeaveStatistics([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var statistics = await _serviceManager.LeaveService.GetLeaveStatisticsAsync(startDate, endDate);
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Export leave report
    /// </summary>
    [HttpGet("export")]
    public async Task<IActionResult> ExportLeaveReport([FromQuery] string format = "excel", [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        try
        {
            if (format != "excel" && format != "pdf")
                return BadRequest("Format must be 'excel' or 'pdf'.");

            var reportData = await _serviceManager.LeaveService.ExportLeaveReportAsync(format, startDate, endDate);
            
            var contentType = format == "excel" ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "application/pdf";
            var fileName = $"leave-report-{DateTime.Now:yyyyMMdd}.{(format == "excel" ? "xlsx" : "pdf")}";
            
            return File(reportData, contentType, fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get leave type label
    /// </summary>
    [HttpGet("types/{type}/label")]
    public IActionResult GetLeaveTypeLabel(LeaveType type)
    {
        try
        {
            var label = _serviceManager.LeaveService.GetLeaveTypeLabel(type);
            return Ok(new { type, label });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get leave status label
    /// </summary>
    [HttpGet("status/{status}/label")]
    public IActionResult GetLeaveStatusLabel(LeaveStatus status)
    {
        try
        {
            var label = _serviceManager.LeaveService.GetLeaveStatusLabel(status);
            return Ok(new { status, label });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Calculate leave days between two dates
    /// </summary>
    [HttpGet("calculate-days")]
    public IActionResult CalculateLeaveDays([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        try
        {
            if (startDate > endDate)
                return BadRequest("Start date cannot be after end date.");

            var days = _serviceManager.LeaveService.CalculateLeaveDays(startDate, endDate);
            return Ok(new { startDate, endDate, days });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}