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

    public override async Task<IEnumerable<RequestDto>> GetAllAsync(RequestParameters parameters, bool trackChanges)
    {
        var requests = await _repository.Request.GetAllRequestsAsync(parameters, trackChanges);
        return _mapper.Map<IEnumerable<RequestDto>>(requests);
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

    public async Task<IEnumerable<RequestDto>> GetRequestsByUserAsync(string userId, RequestParameters parameters, bool trackChanges)
    {
        var requests = await _repository.Request.GetRequestsByUserAsync(userId, parameters, trackChanges);
        return _mapper.Map<IEnumerable<RequestDto>>(requests);
    }

    public async Task<IEnumerable<RequestDto>> GetRequestsByStatusAsync(RequestStatus status, RequestParameters parameters, bool trackChanges)
    {
        var requests = await _repository.Request.GetRequestsByStatusAsync(status, parameters, trackChanges);
        return _mapper.Map<IEnumerable<RequestDto>>(requests);
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

        return _mapper.Map<RequestDto>(request);
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

    public async Task<IEnumerable<RequestDto>> GetPendingRequestsForUserAsync(string userId, RequestParameters parameters, bool trackChanges)
    {
        var requests = await _repository.Request.GetPendingRequestsForUserAsync(userId, parameters, trackChanges);
        return _mapper.Map<IEnumerable<RequestDto>>(requests);
    }
}
