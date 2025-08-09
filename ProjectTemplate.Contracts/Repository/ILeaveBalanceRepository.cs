using ProjectTemplate.Models.Entities;

namespace ProjectTemplate.Contracts.Repository;

public interface ILeaveBalanceRepository : IRepositoryBase<LeaveBalance>
{
    Task<IEnumerable<LeaveBalance>> GetLeaveBalancesByEmployeeIdAsync(string employeeId, bool trackChanges);
    Task<LeaveBalance?> GetLeaveBalanceAsync(string employeeId, LeaveType leaveType, int year, bool trackChanges);
    Task<IEnumerable<LeaveBalance>> GetAllLeaveBalancesAsync(bool trackChanges);
    void CreateLeaveBalance(LeaveBalance leaveBalance);
    void UpdateLeaveBalance(LeaveBalance leaveBalance);
    void DeleteLeaveBalance(LeaveBalance leaveBalance);
}