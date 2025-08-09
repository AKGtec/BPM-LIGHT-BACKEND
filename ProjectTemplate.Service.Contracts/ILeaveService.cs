using ProjectTemplate.Models.Entities;
using ProjectTemplate.Shared.DataTransferObjects;
using ProjectTemplate.Shared.RequestFeatures;

namespace ProjectTemplate.Service.Contracts;

public interface ILeaveService
{
    // Leave Requests
    Task<PaginatedResponse<LeaveRequestDto>> GetLeaveRequestsAsync(LeaveRequestParameters parameters, bool trackChanges);
    Task<PaginatedResponse<LeaveRequestDto>> GetPendingLeaveRequestsAsync(LeaveRequestParameters parameters, bool trackChanges);
    Task<PaginatedResponse<LeaveRequestDto>> GetMyLeaveRequestsAsync(string employeeId, LeaveRequestParameters parameters, bool trackChanges);
    Task<LeaveRequestDto?> GetLeaveRequestByIdAsync(Guid id, bool trackChanges);
    Task<LeaveRequestDto> CreateLeaveRequestAsync(CreateLeaveRequestDto createDto, string currentUserId);
    Task ApproveLeaveRequestAsync(Guid id, ApproveRejectLeaveDto data, string approverId);
    Task RejectLeaveRequestAsync(Guid id, ApproveRejectLeaveDto data, string rejectorId);
    Task CancelLeaveRequestAsync(Guid id, string currentUserId);
    Task BulkApproveLeaveRequestsAsync(BulkApproveRejectLeaveDto data, string approverId);
    Task BulkRejectLeaveRequestsAsync(BulkApproveRejectLeaveDto data, string rejectorId);

    // Leave Balances
    Task<IEnumerable<LeaveBalanceDto>> GetLeaveBalancesAsync(string? employeeId = null);
    Task<IEnumerable<LeaveBalanceDto>> GetMyLeaveBalancesAsync(string employeeId);
    Task UpdateLeaveBalanceAsync(UpdateLeaveBalanceDto updateDto);

    // Leave Types Configuration
    Task<IEnumerable<LeaveTypeConfigDto>> GetLeaveTypesAsync();

    // Statistics and Reports
    Task<LeaveStatisticsDto> GetLeaveStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<byte[]> ExportLeaveReportAsync(string format, DateTime? startDate = null, DateTime? endDate = null);

    // Utility methods
    string GetLeaveTypeLabel(LeaveType type);
    string GetLeaveStatusLabel(LeaveStatus status);
    int CalculateLeaveDays(DateTime startDate, DateTime endDate);
}