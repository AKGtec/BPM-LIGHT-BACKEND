using ProjectTemplate.Models.Entities;
using ProjectTemplate.Shared.RequestFeatures;
using ProjectTemplate.Shared.DataTransferObjects;

namespace ProjectTemplate.Contracts.Repository;

public interface ILeaveRepository : IRepositoryBase<Leave>
{
    Task<PagedList<Leave>> GetAllLeavesAsync(LeaveRequestParameters parameters, bool trackChanges);
    Task<PagedList<Leave>> GetLeavesByEmployeeIdAsync(string employeeId, LeaveRequestParameters parameters, bool trackChanges);
    Task<PagedList<Leave>> GetPendingLeavesAsync(LeaveRequestParameters parameters, bool trackChanges);
    Task<Leave?> GetLeaveByIdAsync(Guid id, bool trackChanges);
    Task<IEnumerable<Leave>> GetLeavesByStatusAsync(LeaveStatus status, bool trackChanges);
    Task<IEnumerable<Leave>> GetLeavesByDateRangeAsync(DateTime startDate, DateTime endDate, bool trackChanges);
    Task<int> GetLeaveCountByEmployeeAndTypeAsync(string employeeId, LeaveType leaveType, int year, bool trackChanges);
    void CreateLeave(Leave leave);
    void UpdateLeave(Leave leave);
    void DeleteLeave(Leave leave);
}