using ProjectTemplate.Models.Entities;
using ProjectTemplate.Shared.RequestFeatures;
using ProjectTemplate.Shared.DataTransferObjects;

namespace ProjectTemplate.Contracts.Repository;

public interface IRequestRepository : IRepositoryBase<Request>
{
    Task<IEnumerable<Request>> GetAllRequestsAsync(RequestParameters parameters, bool trackChanges);
    Task<Request?> GetRequestAsync(Guid requestId, bool trackChanges);
    Task<Request?> GetRequestWithStepsAsync(Guid requestId, bool trackChanges);
    Task<IEnumerable<Request>> GetRequestsByUserAsync(string userId, RequestParameters parameters, bool trackChanges);
    Task<IEnumerable<Request>> GetRequestsByStatusAsync(RequestStatus status, RequestParameters parameters, bool trackChanges);
    Task<IEnumerable<Request>> GetPendingRequestsForUserAsync(string userId, RequestParameters parameters, bool trackChanges);
    Task<int> GetRequestCountAsync();
    Task<int> GetRequestCountByUserAsync(string userId);
    Task<int> GetRequestCountByStatusAsync(RequestStatus status);
    Task<int> GetPendingRequestCountForUserAsync(string userId);
    void CreateRequest(Request request);
    void DeleteRequest(Request request);
    
    // Additional methods for new endpoints
    Task<IEnumerable<Request>> GetRequestsByTypeAsync(RequestType type, RequestParameters parameters, bool trackChanges);
    Task<int> GetRequestCountByTypeAsync(RequestType type);
    Task<IEnumerable<Request>> GetRequestsForRoleAsync(string role, RequestParameters parameters, bool trackChanges);
    Task<int> GetRequestCountForRoleAsync(string role);
    Task<IEnumerable<Request>> GetRequestsByDateRangeAsync(DateTime startDate, DateTime endDate, RequestParameters parameters, bool trackChanges);
    Task<int> GetRequestCountByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Request>> GetRequestsByFilterAsync(RequestFilterDto filter, RequestParameters parameters, bool trackChanges);
    Task<int> GetRequestCountByFilterAsync(RequestFilterDto filter);
}
