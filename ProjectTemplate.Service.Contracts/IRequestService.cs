using ProjectTemplate.Models.Entities;
using ProjectTemplate.Shared.RequestFeatures;
using ProjectTemplate.Shared.DataTransferObjects;

namespace ProjectTemplate.Service.Contracts;

public interface IRequestService : IBaseService<RequestDto, Request>
{
    Task<IEnumerable<RequestDto>> GetRequestsByUserAsync(string userId, RequestParameters parameters, bool trackChanges);
    Task<IEnumerable<RequestDto>> GetRequestsByStatusAsync(RequestStatus status, RequestParameters parameters, bool trackChanges);
    Task<RequestDto?> GetRequestWithStepsAsync(Guid id, bool trackChanges);
    Task<RequestDto> CreateRequestAsync(CreateRequestDto dto, string initiatorId);
    Task<RequestDto> ApproveRequestStepAsync(Guid requestId, Guid stepId, string validatorId, string? comments = null);
    Task<RequestDto> RejectRequestStepAsync(Guid requestId, Guid stepId, string validatorId, string? comments = null);
    Task<IEnumerable<RequestDto>> GetPendingRequestsForUserAsync(string userId, RequestParameters parameters, bool trackChanges);
}
