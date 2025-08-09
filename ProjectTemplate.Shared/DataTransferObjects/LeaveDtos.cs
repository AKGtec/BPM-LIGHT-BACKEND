using ProjectTemplate.Models.Entities;
using ProjectTemplate.Shared.RequestFeatures;

namespace ProjectTemplate.Shared.DataTransferObjects;

public class LeaveRequestDto : BaseDto
{
    public string EmployeeId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeEmail { get; set; } = string.Empty;
    public LeaveType LeaveType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Days { get; set; }
    public string Reason { get; set; } = string.Empty;
    public LeaveStatus Status { get; set; }
    public DateTime SubmittedDate { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string? RejectedBy { get; set; }
    public DateTime? RejectedDate { get; set; }
    public string? Comments { get; set; }
    public List<string> Attachments { get; set; } = new List<string>();
}

public class CreateLeaveRequestDto
{
    public string EmployeeId { get; set; } = string.Empty;
    public LeaveType LeaveType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public List<string>? AttachmentFileNames { get; set; }
}

public class ApproveRejectLeaveDto
{
    public string? Comments { get; set; }
}

public class BulkApproveRejectLeaveDto
{
    public List<Guid> Ids { get; set; } = new List<Guid>();
    public string? Comments { get; set; }
}

public class LeaveBalanceDto
{
    public string EmployeeId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public LeaveType LeaveType { get; set; }
    public int TotalDays { get; set; }
    public int UsedDays { get; set; }
    public int RemainingDays { get; set; }
    public int Year { get; set; }
}

public class LeaveTypeConfigDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MaxDaysPerYear { get; set; }
    public bool RequiresApproval { get; set; }
    public bool AllowCarryOver { get; set; }
    public bool IsActive { get; set; }
}

public class LeaveStatisticsDto
{
    public int TotalRequests { get; set; }
    public int PendingRequests { get; set; }
    public int ApprovedRequests { get; set; }
    public int RejectedRequests { get; set; }
    public int TotalDaysRequested { get; set; }
    public int TotalDaysApproved { get; set; }
    public string MostRequestedLeaveType { get; set; } = string.Empty;
}

public class UpdateLeaveBalanceDto
{
    public string EmployeeId { get; set; } = string.Empty;
    public LeaveType LeaveType { get; set; }
    public int Year { get; set; } = DateTime.UtcNow.Year;
    public int TotalDays { get; set; }
    public int UsedDays { get; set; }
    public int RemainingDays { get; set; }
    public int CarriedForwardDays { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class LeaveRequestParameters : RequestParameters
{
    public string? SearchTerm { get; set; }
    public LeaveStatus? Status { get; set; }
    public LeaveType? Type { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? EmployeeId { get; set; }
}