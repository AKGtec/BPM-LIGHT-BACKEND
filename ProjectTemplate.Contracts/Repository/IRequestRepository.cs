using ProjectTemplate.Models.Entities;
using ProjectTemplate.Shared.RequestFeatures;

namespace ProjectTemplate.Contracts.Repository;

public interface IRequestRepository : IRepositoryBase<Request>
{
    Task<IEnumerable<Request>> GetAllRequestsAsync(RequestParameters parameters, bool trackChanges);
    Task<Request?> GetRequestAsync(Guid requestId, bool trackChanges);
    Task<Request?> GetRequestWithStepsAsync(Guid requestId, bool trackChanges);
    Task<IEnumerable<Request>> GetRequestsByUserAsync(string userId, RequestParameters parameters, bool trackChanges);
    Task<IEnumerable<Request>> GetRequestsByStatusAsync(RequestStatus status, RequestParameters parameters, bool trackChanges);
    Task<IEnumerable<Request>> GetPendingRequestsForUserAsync(string userId, RequestParameters parameters, bool trackChanges);
    void CreateRequest(Request request);
    void DeleteRequest(Request request);
}
