using Microsoft.EntityFrameworkCore;
using ProjectTemplate.Contracts.Repository;
using ProjectTemplate.Models.DataSource;
using ProjectTemplate.Models.Entities;
using ProjectTemplate.Shared.RequestFeatures;
using ProjectTemplate.Shared.DataTransferObjects;

namespace ProjectTemplate.Repository.Repository;

public sealed class LeaveRepository : RepositoryBase<Leave>, ILeaveRepository
{
    public LeaveRepository(ProjectTemplateContext repositoryContext) : base(repositoryContext)
    {
    }

    public async Task<PagedList<Leave>> GetAllLeavesAsync(LeaveRequestParameters parameters, bool trackChanges)
    {
        var query = FindAll(trackChanges)
            .Include(l => l.Employee)
            .Include(l => l.Approver)
            .Include(l => l.Rejector)
            .AsQueryable();

        // Apply filters
        query = ApplyFilters(query, parameters);

        // Apply sorting
        query = ApplySorting(query, parameters.SortBy, parameters.SortDirection);

        return await PagedList<Leave>.ToPagedListAsync(query, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<PagedList<Leave>> GetLeavesByEmployeeIdAsync(string employeeId, LeaveRequestParameters parameters, bool trackChanges)
    {
        var query = FindByCondition(l => l.EmployeeId == employeeId, trackChanges)
            .Include(l => l.Employee)
            .Include(l => l.Approver)
            .Include(l => l.Rejector)
            .AsQueryable();

        // Apply filters
        query = ApplyFilters(query, parameters);

        // Apply sorting
        query = ApplySorting(query, parameters.SortBy, parameters.SortDirection);

        return await PagedList<Leave>.ToPagedListAsync(query, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<PagedList<Leave>> GetPendingLeavesAsync(LeaveRequestParameters parameters, bool trackChanges)
    {
        var query = FindByCondition(l => l.Status == LeaveStatus.Pending, trackChanges)
            .Include(l => l.Employee)
            .Include(l => l.Approver)
            .Include(l => l.Rejector)
            .AsQueryable();

        // Apply filters
        query = ApplyFilters(query, parameters);

        // Apply sorting
        query = ApplySorting(query, parameters.SortBy, parameters.SortDirection);

        return await PagedList<Leave>.ToPagedListAsync(query, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<Leave?> GetLeaveByIdAsync(Guid id, bool trackChanges)
    {
        return await FindByCondition(l => l.Id == id, trackChanges)
            .Include(l => l.Employee)
            .Include(l => l.Approver)
            .Include(l => l.Rejector)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Leave>> GetLeavesByStatusAsync(LeaveStatus status, bool trackChanges)
    {
        return await FindByCondition(l => l.Status == status, trackChanges)
            .Include(l => l.Employee)
            .ToListAsync();
    }

    public async Task<IEnumerable<Leave>> GetLeavesByDateRangeAsync(DateTime startDate, DateTime endDate, bool trackChanges)
    {
        return await FindByCondition(l => l.StartDate >= startDate && l.EndDate <= endDate, trackChanges)
            .Include(l => l.Employee)
            .ToListAsync();
    }

    public async Task<int> GetLeaveCountByEmployeeAndTypeAsync(string employeeId, LeaveType leaveType, int year, bool trackChanges)
    {
        return await FindByCondition(l => l.EmployeeId == employeeId && 
                                         l.LeaveType == leaveType && 
                                         l.StartDate.Year == year &&
                                         l.Status == LeaveStatus.Approved, trackChanges)
            .SumAsync(l => l.Days);
    }

    public void CreateLeave(Leave leave) => Create(leave);

    public void UpdateLeave(Leave leave) => Update(leave);

    public void DeleteLeave(Leave leave) => Delete(leave);

    private IQueryable<Leave> ApplyFilters(IQueryable<Leave> query, LeaveRequestParameters parameters)
    {
        if (!string.IsNullOrEmpty(parameters.SearchTerm))
        {
            var searchTerm = parameters.SearchTerm.ToLower();
            query = query.Where(l => l.Employee.UserName!.ToLower().Contains(searchTerm) ||
                                    l.Employee.Email!.ToLower().Contains(searchTerm) ||
                                    l.Reason.ToLower().Contains(searchTerm));
        }

        if (parameters.Status.HasValue)
        {
            query = query.Where(l => l.Status == parameters.Status.Value);
        }

        if (parameters.Type.HasValue)
        {
            query = query.Where(l => l.LeaveType == parameters.Type.Value);
        }

        if (parameters.StartDate.HasValue)
        {
            query = query.Where(l => l.StartDate >= parameters.StartDate.Value);
        }

        if (parameters.EndDate.HasValue)
        {
            query = query.Where(l => l.EndDate <= parameters.EndDate.Value);
        }

        if (!string.IsNullOrEmpty(parameters.EmployeeId))
        {
            query = query.Where(l => l.EmployeeId == parameters.EmployeeId);
        }

        return query;
    }

    private IQueryable<Leave> ApplySorting(IQueryable<Leave> query, string? sortBy, string? sortDirection)
    {
        var isDescending = sortDirection?.ToLower() == "desc";

        return sortBy?.ToLower() switch
        {
            "submitteddate" => isDescending ? query.OrderByDescending(l => l.SubmittedDate) : query.OrderBy(l => l.SubmittedDate),
            "startdate" => isDescending ? query.OrderByDescending(l => l.StartDate) : query.OrderBy(l => l.StartDate),
            "enddate" => isDescending ? query.OrderByDescending(l => l.EndDate) : query.OrderBy(l => l.EndDate),
            "days" => isDescending ? query.OrderByDescending(l => l.Days) : query.OrderBy(l => l.Days),
            "status" => isDescending ? query.OrderByDescending(l => l.Status) : query.OrderBy(l => l.Status),
            "leavetype" => isDescending ? query.OrderByDescending(l => l.LeaveType) : query.OrderBy(l => l.LeaveType),
            "employeename" => isDescending ? query.OrderByDescending(l => l.Employee.UserName) : query.OrderBy(l => l.Employee.UserName),
            _ => query.OrderByDescending(l => l.SubmittedDate)
        };
    }
}