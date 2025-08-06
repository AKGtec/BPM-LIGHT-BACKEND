using Microsoft.EntityFrameworkCore;
using ProjectTemplate.Contracts.Repository;
using ProjectTemplate.Models.DataSource;
using ProjectTemplate.Models.Entities;
using ProjectTemplate.Shared.RequestFeatures;

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
        return await FindAll(trackChanges)
            .Include(r => r.Initiator)
            .Include(r => r.RequestSteps)
                .ThenInclude(rs => rs.WorkflowStep)
            .Where(r => r.RequestSteps.Any(rs => 
                rs.Status == StepStatus.Pending && 
                rs.WorkflowStep.ResponsibleRole == userId)) // Assuming userId contains role for simplicity
            .OrderByDescending(r => r.CreatedAt)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();
    }

    public void CreateRequest(Request request) => Create(request);

    public void DeleteRequest(Request request) => Delete(request);
}
