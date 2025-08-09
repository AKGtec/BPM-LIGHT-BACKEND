using AutoMapper;
using ProjectTemplate.Contracts;
using ProjectTemplate.Contracts.Repository;
using ProjectTemplate.Models.Entities;
using ProjectTemplate.Service.Contracts;
using ProjectTemplate.Shared.DataTransferObjects;
using ProjectTemplate.Shared.RequestFeatures;

namespace ProjectTemplate.Service;

public class RequestService : BaseService<RequestDto, Request>, IRequestService
{
    public RequestService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        : base(repository, logger, mapper)
    {
    }

    public override async Task<PaginatedResponse<RequestDto>> GetAllAsync(RequestParameters parameters, bool trackChanges)
    {
        var requests = await _repository.Request.GetAllRequestsAsync(parameters, trackChanges);
        var totalCount = await _repository.Request.GetRequestCountAsync();

        var requestDtos = _mapper.Map<IEnumerable<RequestDto>>(requests);

        return new PaginatedResponse<RequestDto>(requestDtos, totalCount, parameters.PageNumber, parameters.PageSize);
    }

    public override async Task<RequestDto?> GetByIdAsync(Guid id, bool trackChanges)
    {
        var request = await _repository.Request.GetRequestAsync(id, trackChanges);
        return request == null ? null : _mapper.Map<RequestDto>(request);
    }

    public override async Task<RequestDto> CreateAsync(RequestDto dto)
    {
        var request = _mapper.Map<Request>(dto);
        _repository.Request.CreateRequest(request);
        await _repository.SaveAsync();

        return _mapper.Map<RequestDto>(request);
    }

    public override async Task UpdateAsync(Guid id, RequestDto dto, bool trackChanges)
    {
        var request = await _repository.Request.GetRequestAsync(id, trackChanges);
        CheckIfEntityExists(request, id);

        _mapper.Map(dto, request);
        await _repository.SaveAsync();
    }

    public override async Task DeleteAsync(Guid id, bool trackChanges)
    {
        var request = await _repository.Request.GetRequestAsync(id, trackChanges);
        CheckIfEntityExists(request, id);

        _repository.Request.DeleteRequest(request!);
        await _repository.SaveAsync();
    }

    public async Task<PaginatedResponse<RequestDto>> GetRequestsByUserAsync(string userId, RequestParameters parameters, bool trackChanges)
    {
        var requests = await _repository.Request.GetRequestsByUserAsync(userId, parameters, trackChanges);
        var totalCount = await _repository.Request.GetRequestCountByUserAsync(userId);

        var requestDtos = _mapper.Map<IEnumerable<RequestDto>>(requests);

        return new PaginatedResponse<RequestDto>(requestDtos, totalCount, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<PaginatedResponse<RequestDto>> GetRequestsByStatusAsync(RequestStatus status, RequestParameters parameters, bool trackChanges)
    {
        var requests = await _repository.Request.GetRequestsByStatusAsync(status, parameters, trackChanges);
        var totalCount = await _repository.Request.GetRequestCountByStatusAsync(status);

        var requestDtos = _mapper.Map<IEnumerable<RequestDto>>(requests);

        return new PaginatedResponse<RequestDto>(requestDtos, totalCount, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<RequestDto?> GetRequestWithStepsAsync(Guid id, bool trackChanges)
    {
        var request = await _repository.Request.GetRequestWithStepsAsync(id, trackChanges);
        return request == null ? null : _mapper.Map<RequestDto>(request);
    }

    public async Task<RequestDto> CreateRequestAsync(CreateRequestDto dto, string initiatorId)
    {
        // Get the workflow to create request steps
        var workflow = await _repository.Workflow.GetWorkflowWithStepsAsync(dto.WorkflowId, false);
        if (workflow == null)
            throw new ArgumentException($"Workflow with ID {dto.WorkflowId} not found");

        var request = new Request
        {
            Type = dto.Type,
            Description = dto.Description,
            Title = dto.Title,
            InitiatorId = initiatorId,
            Status = RequestStatus.Pending
        };

        // Create request steps based on workflow steps
        foreach (var workflowStep in workflow.Steps.OrderBy(s => s.Order))
        {
            request.RequestSteps.Add(new RequestStep
            {
                WorkflowStepId = workflowStep.Id,
                Status = StepStatus.Pending
            });
        }

        _repository.Request.CreateRequest(request);
        await _repository.SaveAsync();

        // IMPORTANT: Reload the request with steps from database to get all populated properties
        var savedRequest = await _repository.Request.GetRequestWithStepsAsync(request.Id, false);

        return _mapper.Map<RequestDto>(savedRequest);
    }

    public async Task<RequestDto> ApproveRequestStepAsync(Guid requestId, Guid stepId, string validatorId, string? comments = null)
    {
        var request = await _repository.Request.GetRequestWithStepsAsync(requestId, true);
        CheckIfEntityExists(request, requestId);

        var requestStep = request!.RequestSteps.FirstOrDefault(rs => rs.Id == stepId);
        if (requestStep == null)
            throw new ArgumentException($"Request step with ID {stepId} not found");

        if (requestStep.Status != StepStatus.Pending)
            throw new InvalidOperationException("Only pending steps can be approved");

        requestStep.Status = StepStatus.Approved;
        requestStep.ValidatedAt = DateTime.UtcNow;
        requestStep.ValidatorId = validatorId;
        requestStep.Comments = comments;

        // Check if all steps are approved
        if (request.RequestSteps.All(rs => rs.Status == StepStatus.Approved))
        {
            request.Status = RequestStatus.Approved;
        }

        await _repository.SaveAsync();
        return _mapper.Map<RequestDto>(request);
    }

    public async Task<RequestDto> RejectRequestStepAsync(Guid requestId, Guid stepId, string validatorId, string? comments = null)
    {
        var request = await _repository.Request.GetRequestWithStepsAsync(requestId, true);
        CheckIfEntityExists(request, requestId);

        var requestStep = request!.RequestSteps.FirstOrDefault(rs => rs.Id == stepId);
        if (requestStep == null)
            throw new ArgumentException($"Request step with ID {stepId} not found");

        if (requestStep.Status != StepStatus.Pending)
            throw new InvalidOperationException("Only pending steps can be rejected");

        requestStep.Status = StepStatus.Rejected;
        requestStep.ValidatedAt = DateTime.UtcNow;
        requestStep.ValidatorId = validatorId;
        requestStep.Comments = comments;

        // Reject the entire request
        request.Status = RequestStatus.Rejected;

        await _repository.SaveAsync();
        return _mapper.Map<RequestDto>(request);
    }

    public async Task<PaginatedResponse<RequestDto>> GetPendingRequestsForUserAsync(string userId, RequestParameters parameters, bool trackChanges)
    {
        var requests = await _repository.Request.GetPendingRequestsForUserAsync(userId, parameters, trackChanges);
        var totalCount = await _repository.Request.GetPendingRequestCountForUserAsync(userId);

        var requestDtos = _mapper.Map<IEnumerable<RequestDto>>(requests);

        return new PaginatedResponse<RequestDto>(requestDtos, totalCount, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<RequestSummaryDto> GetRequestSummaryAsync()
    {
        var totalRequests = await _repository.Request.GetRequestCountAsync();
        var pendingRequests = await _repository.Request.GetRequestCountByStatusAsync(RequestStatus.Pending);
        var approvedRequests = await _repository.Request.GetRequestCountByStatusAsync(RequestStatus.Approved);
        var rejectedRequests = await _repository.Request.GetRequestCountByStatusAsync(RequestStatus.Rejected);
        var archivedRequests = await _repository.Request.GetRequestCountByStatusAsync(RequestStatus.Archived);

        var requestsByType = new Dictionary<RequestType, int>();
        foreach (RequestType type in Enum.GetValues<RequestType>())
        {
            var count = await _repository.Request.GetRequestCountByTypeAsync(type);
            requestsByType[type] = count;
        }

        var requestsByStatus = new Dictionary<RequestStatus, int>
        {
            { RequestStatus.Pending, pendingRequests },
            { RequestStatus.Approved, approvedRequests },
            { RequestStatus.Rejected, rejectedRequests },
            { RequestStatus.Archived, archivedRequests }
        };

        return new RequestSummaryDto
        {
            TotalRequests = totalRequests,
            PendingRequests = pendingRequests,
            ApprovedRequests = approvedRequests,
            RejectedRequests = rejectedRequests,
            ArchivedRequests = archivedRequests,
            RequestsByType = requestsByType,
            RequestsByStatus = requestsByStatus
        };
    }

    public async Task<PaginatedResponse<RequestDto>> GetRequestsByTypeAsync(RequestType type, RequestParameters parameters, bool trackChanges)
    {
        var requests = await _repository.Request.GetRequestsByTypeAsync(type, parameters, trackChanges);
        var totalCount = await _repository.Request.GetRequestCountByTypeAsync(type);

        var requestDtos = _mapper.Map<IEnumerable<RequestDto>>(requests);

        return new PaginatedResponse<RequestDto>(requestDtos, totalCount, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<PaginatedResponse<RequestDto>> GetRequestsForRoleAsync(string role, RequestParameters parameters, bool trackChanges)
    {
        var requests = await _repository.Request.GetRequestsForRoleAsync(role, parameters, trackChanges);
        var totalCount = await _repository.Request.GetRequestCountForRoleAsync(role);

        var requestDtos = _mapper.Map<IEnumerable<RequestDto>>(requests);

        return new PaginatedResponse<RequestDto>(requestDtos, totalCount, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<PaginatedResponse<RequestDto>> GetRequestsByDateRangeAsync(DateTime startDate, DateTime endDate, RequestParameters parameters, bool trackChanges)
    {
        var requests = await _repository.Request.GetRequestsByDateRangeAsync(startDate, endDate, parameters, trackChanges);
        var totalCount = await _repository.Request.GetRequestCountByDateRangeAsync(startDate, endDate);

        var requestDtos = _mapper.Map<IEnumerable<RequestDto>>(requests);

        return new PaginatedResponse<RequestDto>(requestDtos, totalCount, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<PaginatedResponse<RequestDto>> GetRequestsByFilterAsync(RequestFilterDto filter, RequestParameters parameters, bool trackChanges)
    {
        var requests = await _repository.Request.GetRequestsByFilterAsync(filter, parameters, trackChanges);
        var totalCount = await _repository.Request.GetRequestCountByFilterAsync(filter);

        var requestDtos = _mapper.Map<IEnumerable<RequestDto>>(requests);

        return new PaginatedResponse<RequestDto>(requestDtos, totalCount, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<RequestDto> CancelRequestAsync(Guid id, string userId, string? reason = null)
    {
        var request = await _repository.Request.GetRequestAsync(id, true);
        CheckIfEntityExists(request, id);

        if (request!.Status != RequestStatus.Pending)
            throw new InvalidOperationException("Only pending requests can be cancelled");

        if (request.InitiatorId != userId)
            throw new UnauthorizedAccessException("Only the request initiator can cancel the request");

        request.Status = RequestStatus.Rejected; // Using Rejected as cancelled status
        request.UpdatedAt = DateTime.UtcNow;

        // Add to history if needed
        await _repository.SaveAsync();

        return _mapper.Map<RequestDto>(request);
    }

    public async Task ArchiveRequestAsync(Guid id, string userId)
    {
        var request = await _repository.Request.GetRequestAsync(id, true);
        CheckIfEntityExists(request, id);

        request!.Status = RequestStatus.Archived;
        request.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveAsync();
    }

    public async Task RestoreRequestAsync(Guid id, string userId)
    {
        var request = await _repository.Request.GetRequestAsync(id, true);
        CheckIfEntityExists(request, id);

        if (request!.Status != RequestStatus.Archived)
            throw new InvalidOperationException("Only archived requests can be restored");

        request.Status = RequestStatus.Pending;
        request.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveAsync();
    }

    public async Task<List<RequestHistoryDto>> GetRequestHistoryAsync(Guid id)
    {
        // For now, return empty list - this would need a separate RequestHistory entity
        return new List<RequestHistoryDto>();
    }

    public async Task<BulkActionResultDto> BulkApproveRequestsAsync(List<Guid> requestIds, string userId, string? comments = null)
    {
        var result = new BulkActionResultDto();
        
        foreach (var requestId in requestIds)
        {
            try
            {
                var request = await _repository.Request.GetRequestWithStepsAsync(requestId, true);
                if (request != null)
                {
                    // Find the next pending step for this user
                    var pendingStep = request.RequestSteps
                        .Where(rs => rs.Status == StepStatus.Pending)
                        .OrderBy(rs => rs.WorkflowStep.Order)
                        .FirstOrDefault();

                    if (pendingStep != null)
                    {
                        await ApproveRequestStepAsync(requestId, pendingStep.Id, userId, comments);
                        result.SuccessCount++;
                        result.ProcessedRequestIds.Add(requestId);
                    }
                    else
                    {
                        result.FailureCount++;
                        result.Errors.Add($"No pending steps found for request {requestId}");
                    }
                }
                else
                {
                    result.FailureCount++;
                    result.Errors.Add($"Request {requestId} not found");
                }
            }
            catch (Exception ex)
            {
                result.FailureCount++;
                result.Errors.Add($"Error processing request {requestId}: {ex.Message}");
            }
        }

        return result;
    }

    public async Task<BulkActionResultDto> BulkRejectRequestsAsync(List<Guid> requestIds, string userId, string? comments = null)
    {
        var result = new BulkActionResultDto();
        
        foreach (var requestId in requestIds)
        {
            try
            {
                var request = await _repository.Request.GetRequestWithStepsAsync(requestId, true);
                if (request != null)
                {
                    // Find the next pending step for this user
                    var pendingStep = request.RequestSteps
                        .Where(rs => rs.Status == StepStatus.Pending)
                        .OrderBy(rs => rs.WorkflowStep.Order)
                        .FirstOrDefault();

                    if (pendingStep != null)
                    {
                        await RejectRequestStepAsync(requestId, pendingStep.Id, userId, comments);
                        result.SuccessCount++;
                        result.ProcessedRequestIds.Add(requestId);
                    }
                    else
                    {
                        result.FailureCount++;
                        result.Errors.Add($"No pending steps found for request {requestId}");
                    }
                }
                else
                {
                    result.FailureCount++;
                    result.Errors.Add($"Request {requestId} not found");
                }
            }
            catch (Exception ex)
            {
                result.FailureCount++;
                result.Errors.Add($"Error processing request {requestId}: {ex.Message}");
            }
        }

        return result;
    }

    public async Task<RequestStatisticsDto> GetRequestStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var totalRequests = await _repository.Request.GetRequestCountAsync();
        var pendingRequests = await _repository.Request.GetRequestCountByStatusAsync(RequestStatus.Pending);
        var approvedRequests = await _repository.Request.GetRequestCountByStatusAsync(RequestStatus.Approved);
        var rejectedRequests = await _repository.Request.GetRequestCountByStatusAsync(RequestStatus.Rejected);

        var requestsByType = new Dictionary<RequestType, int>();
        foreach (RequestType type in Enum.GetValues<RequestType>())
        {
            var count = await _repository.Request.GetRequestCountByTypeAsync(type);
            requestsByType[type] = count;
        }

        return new RequestStatisticsDto
        {
            TotalRequests = totalRequests,
            PendingRequests = pendingRequests,
            ApprovedRequests = approvedRequests,
            RejectedRequests = rejectedRequests,
            CancelledRequests = 0, // Would need separate tracking
            AverageProcessingTime = 0, // Would need calculation based on timestamps
            RequestsByType = requestsByType,
            RequestsByMonth = new Dictionary<string, int>(), // Would need date-based queries
            AverageProcessingTimeByType = new Dictionary<string, double>(), // Would need calculation
            TopInitiators = new List<TopInitiatorDto>() // Would need aggregation query
        };
    }
}
