using Microsoft.EntityFrameworkCore;
using ProjectTemplate.Contracts.Repository;
using ProjectTemplate.Models.DataSource;
using ProjectTemplate.Models.Entities;

namespace ProjectTemplate.Repository.Repository;

public sealed class LeaveBalanceRepository : RepositoryBase<LeaveBalance>, ILeaveBalanceRepository
{
    public LeaveBalanceRepository(ProjectTemplateContext repositoryContext) : base(repositoryContext)
    {
    }

    public async Task<IEnumerable<LeaveBalance>> GetLeaveBalancesByEmployeeIdAsync(string employeeId, bool trackChanges)
    {
        return await FindByCondition(lb => lb.EmployeeId == employeeId, trackChanges)
            .Include(lb => lb.Employee)
            .OrderBy(lb => lb.LeaveType)
            .ToListAsync();
    }

    public async Task<LeaveBalance?> GetLeaveBalanceAsync(string employeeId, LeaveType leaveType, int year, bool trackChanges)
    {
        return await FindByCondition(lb => lb.EmployeeId == employeeId && 
                                          lb.LeaveType == leaveType && 
                                          lb.Year == year, trackChanges)
            .Include(lb => lb.Employee)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<LeaveBalance>> GetAllLeaveBalancesAsync(bool trackChanges)
    {
        return await FindAll(trackChanges)
            .Include(lb => lb.Employee)
            .OrderBy(lb => lb.Employee.UserName)
            .ThenBy(lb => lb.LeaveType)
            .ToListAsync();
    }

    public void CreateLeaveBalance(LeaveBalance leaveBalance) => Create(leaveBalance);

    public void UpdateLeaveBalance(LeaveBalance leaveBalance) => Update(leaveBalance);

    public void DeleteLeaveBalance(LeaveBalance leaveBalance) => Delete(leaveBalance);
}