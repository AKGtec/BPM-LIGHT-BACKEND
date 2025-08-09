using ProjectTemplate.Models.Entities;
using ProjectTemplate.Shared.RequestFeatures;
using ProjectTemplate.Shared.DataTransferObjects;

namespace ProjectTemplate.Service.Contracts;

public interface IRequestService : IBaseService<RequestDto, Request>
{
    Task<PaginatedResponse<RequestDto>> GetRequestsByUserAsync(string userId, RequestParameters parameters, bool trackChanges);
    Task<PaginatedResponse<RequestDto>> GetRequestsByStatusAsync(RequestStatus status, RequestParameters parameters, bool trackChanges);
    Task<RequestDto?> GetRequestWithStepsAsync(Guid id, bool trackChanges);
    Task<RequestDto> CreateRequestAsync(CreateRequestDto dto, string initiatorId);
    Task<RequestDto> ApproveRequestStepAsync(Guid requestId, Guid stepId, string validatorId, string? comments = null);
    Task<RequestDto> RejectRequestStepAsync(Guid requestId, Guid stepId, string validatorId, string? comments = null);
    Task<PaginatedResponse<RequestDto>> GetPendingRequestsForUserAsync(string userId, RequestParameters parameters, bool trackChanges);
    
    // Additional methods for the new endpoints
    Task<RequestSummaryDto> GetRequestSummaryAsync();
    Task<PaginatedResponse<RequestDto>> GetRequestsByTypeAsync(RequestType type, RequestParameters parameters, bool trackChanges);
    Task<PaginatedResponse<RequestDto>> GetRequestsForRoleAsync(string role, RequestParameters parameters, bool trackChanges);
    Task<PaginatedResponse<RequestDto>> GetRequestsByDateRangeAsync(DateTime startDate, DateTime endDate, RequestParameters parameters, bool trackChanges);
    Task<PaginatedResponse<RequestDto>> GetRequestsByFilterAsync(RequestFilterDto filter, RequestParameters parameters, bool trackChanges);
    Task<RequestDto> CancelRequestAsync(Guid id, string userId, string? reason = null);
    Task ArchiveRequestAsync(Guid id, string userId);
    Task RestoreRequestAsync(Guid id, string userId);
    Task<List<RequestHistoryDto>> GetRequestHistoryAsync(Guid id);
    Task<BulkActionResultDto> BulkApproveRequestsAsync(List<Guid> requestIds, string userId, string? comments = null);
    Task<BulkActionResultDto> BulkRejectRequestsAsync(List<Guid> requestIds, string userId, string? comments = null);
    Task<RequestStatisticsDto> GetRequestStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
}
