using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectTemplate.Contracts.Repository;
using ProjectTemplate.Models.DataSource;
using ProjectTemplate.Models.Entities;
using ProjectTemplate.Shared.RequestFeatures;
using ProjectTemplate.Shared.DataTransferObjects;

namespace ProjectTemplate.Repository.Repository;

public class RequestRepository : RepositoryBase<Request>, IRequestRepository
{
    public RequestRepository(ProjectTemplateContext repositoryContext) : base(repositoryContext)
    {
    }

    public async Task<IEnumerable<Request>> GetAllRequestsAsync(RequestParameters parameters, bool trackChanges)
    {
        return await FindAll(trackChanges)
            .Include(r => r.Initiator)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();
    }

    public async Task<Request?> GetRequestAsync(Guid requestId, bool trackChanges)
    {
        return await FindByCondition(r => r.Id.Equals(requestId), trackChanges)
            .Include(r => r.Initiator)
            .SingleOrDefaultAsync();
    }

    public async Task<Request?> GetRequestWithStepsAsync(Guid requestId, bool trackChanges)
    {
        return await FindByCondition(r => r.Id.Equals(requestId), trackChanges)
            .Include(r => r.Initiator)
            .Include(r => r.RequestSteps)
                .ThenInclude(rs => rs.WorkflowStep)
            .Include(r => r.RequestSteps)
                .ThenInclude(rs => rs.Validator)
            .SingleOrDefaultAsync();
    }

    public async Task<IEnumerable<Request>> GetRequestsByUserAsync(string userId, RequestParameters parameters, bool trackChanges)
    {
        return await FindByCondition(r => r.InitiatorId.Equals(userId), trackChanges)
            .Include(r => r.Initiator)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Request>> GetRequestsByStatusAsync(RequestStatus status, RequestParameters parameters, bool trackChanges)
    {
        return await FindByCondition(r => r.Status == status, trackChanges)
            .Include(r => r.Initiator)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Request>> GetPendingRequestsForUserAsync(string userId, RequestParameters parameters, bool trackChanges)
    {
        // Get user's roles
        var userRoles = await GetUserRolesAsync(userId);
        
        return await FindAll(trackChanges)
            .Include(r => r.Initiator)
            .Include(r => r.RequestSteps)
                .ThenInclude(rs => rs.WorkflowStep)
            .Where(r => r.RequestSteps.Any(rs => 
                rs.Status == StepStatus.Pending && 
                userRoles.Contains(rs.WorkflowStep.ResponsibleRole)))
            .OrderByDescending(r => r.CreatedAt)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();
    }

    public void CreateRequest(Request request) => Create(request);

    public void DeleteRequest(Request request) => Delete(request);

    public async Task<int> GetRequestCountAsync()
    {
        return await FindAll(false).CountAsync();
    }

    public async Task<int> GetRequestCountByUserAsync(string userId)
    {
        return await FindByCondition(r => r.InitiatorId == userId, false).CountAsync();
    }

    public async Task<int> GetRequestCountByStatusAsync(RequestStatus status)
    {
        return await FindByCondition(r => r.Status == status, false).CountAsync();
    }

    public async Task<int> GetPendingRequestCountForUserAsync(string userId)
    {
        // Get user's roles
        var userRoles = await GetUserRolesAsync(userId);
        
        return await FindAll(false)
            .Include(r => r.RequestSteps)
                .ThenInclude(rs => rs.WorkflowStep)
            .Where(r => r.RequestSteps.Any(rs => 
                rs.Status == StepStatus.Pending && 
                userRoles.Contains(rs.WorkflowStep.ResponsibleRole)))
            .CountAsync();
    }

    private async Task<List<string>> GetUserRolesAsync(string userId)
    {
        // Get user's roles from ASP.NET Core Identity system
        return await RepositoryContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .Join(RepositoryContext.Roles,
                ur => ur.RoleId,
                r => r.Id,
                (ur, r) => r.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Request>> GetRequestsByTypeAsync(RequestType type, RequestParameters parameters, bool trackChanges)
    {
        var query = FindByCondition(r => r.Type == type, trackChanges)
            .Include(r => r.Initiator)
            .Include(r => r.RequestSteps)
                .ThenInclude(rs => rs.WorkflowStep)
            .OrderByDescending(r => r.CreatedAt);

        return await PagedList<Request>.ToPagedListAsync(query, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<int> GetRequestCountByTypeAsync(RequestType type)
    {
        return await FindByCondition(r => r.Type == type, false).CountAsync();
    }

    public async Task<IEnumerable<Request>> GetRequestsForRoleAsync(string role, RequestParameters parameters, bool trackChanges)
    {
        var query = FindAll(trackChanges)
            .Include(r => r.Initiator)
            .Include(r => r.RequestSteps)
                .ThenInclude(rs => rs.WorkflowStep)
            .Where(r => r.RequestSteps.Any(rs => rs.WorkflowStep.ResponsibleRole == role))
            .OrderByDescending(r => r.CreatedAt);

        return await PagedList<Request>.ToPagedListAsync(query, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<int> GetRequestCountForRoleAsync(string role)
    {
        return await FindAll(false)
            .Include(r => r.RequestSteps)
                .ThenInclude(rs => rs.WorkflowStep)
            .Where(r => r.RequestSteps.Any(rs => rs.WorkflowStep.ResponsibleRole == role))
            .CountAsync();
    }

    public async Task<IEnumerable<Request>> GetRequestsByDateRangeAsync(DateTime startDate, DateTime endDate, RequestParameters parameters, bool trackChanges)
    {
        var query = FindByCondition(r => r.CreatedAt >= startDate && r.CreatedAt <= endDate, trackChanges)
            .Include(r => r.Initiator)
            .Include(r => r.RequestSteps)
                .ThenInclude(rs => rs.WorkflowStep)
            .OrderByDescending(r => r.CreatedAt);

        return await PagedList<Request>.ToPagedListAsync(query, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<int> GetRequestCountByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await FindByCondition(r => r.CreatedAt >= startDate && r.CreatedAt <= endDate, false).CountAsync();
    }

    public async Task<IEnumerable<Request>> GetRequestsByFilterAsync(RequestFilterDto filter, RequestParameters parameters, bool trackChanges)
    {
        IQueryable<Request> query = FindAll(trackChanges)
            .Include(r => r.Initiator)
            .Include(r => r.RequestSteps)
                .ThenInclude(rs => rs.WorkflowStep);

        // Apply filters
        if (filter.StartDate.HasValue)
            query = query.Where(r => r.CreatedAt >= filter.StartDate.Value);

        if (filter.EndDate.HasValue)
            query = query.Where(r => r.CreatedAt <= filter.EndDate.Value);

        if (filter.Type.HasValue)
            query = query.Where(r => r.Type == filter.Type.Value);

        if (filter.Status.HasValue)
            query = query.Where(r => r.Status == filter.Status.Value);

        if (!string.IsNullOrEmpty(filter.InitiatorId))
            query = query.Where(r => r.InitiatorId == filter.InitiatorId);

        if (!string.IsNullOrEmpty(filter.SearchTerm))
        {
            query = query.Where(r => 
                (r.Title != null && r.Title.Contains(filter.SearchTerm)) || 
                (r.Description != null && r.Description.Contains(filter.SearchTerm)));
        }

        query = query.OrderByDescending(r => r.CreatedAt);

        return await PagedList<Request>.ToPagedListAsync(query, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<int> GetRequestCountByFilterAsync(RequestFilterDto filter)
    {
        IQueryable<Request> query = FindAll(false);

        // Apply same filters as above
        if (filter.StartDate.HasValue)
            query = query.Where(r => r.CreatedAt >= filter.StartDate.Value);

        if (filter.EndDate.HasValue)
            query = query.Where(r => r.CreatedAt <= filter.EndDate.Value);

        if (filter.Type.HasValue)
            query = query.Where(r => r.Type == filter.Type.Value);

        if (filter.Status.HasValue)
            query = query.Where(r => r.Status == filter.Status.Value);

        if (!string.IsNullOrEmpty(filter.InitiatorId))
            query = query.Where(r => r.InitiatorId == filter.InitiatorId);

        if (!string.IsNullOrEmpty(filter.SearchTerm))
        {
            query = query.Where(r => 
                (r.Title != null && r.Title.Contains(filter.SearchTerm)) || 
                (r.Description != null && r.Description.Contains(filter.SearchTerm)));
        }

        return await query.CountAsync();
    }
}
