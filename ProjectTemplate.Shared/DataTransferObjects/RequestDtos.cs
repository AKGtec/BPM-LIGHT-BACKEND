using ProjectTemplate.Models.Entities;

namespace ProjectTemplate.Shared.DataTransferObjects;

public class RequestDto : BaseDto
{
    public RequestType Type { get; set; }
    public string InitiatorId { get; set; } = string.Empty;
    public string InitiatorName { get; set; } = string.Empty;
    public RequestStatus Status { get; set; }
    public string? Description { get; set; }
    public string? Title { get; set; }
    public ICollection<RequestStepDto> RequestSteps { get; set; } = new List<RequestStepDto>();
}

public class CreateRequestDto
{
    public RequestType Type { get; set; }
    public string? Description { get; set; }
    public string? Title { get; set; }
    public Guid WorkflowId { get; set; }
}

public class UpdateRequestDto
{
    public RequestType Type { get; set; }
    public string? Description { get; set; }
    public string? Title { get; set; }
    public RequestStatus Status { get; set; }
}

public class RequestStepDto : BaseDto
{
    public Guid RequestId { get; set; }
    public Guid WorkflowStepId { get; set; }
    public string WorkflowStepName { get; set; } = string.Empty;
    public string ResponsibleRole { get; set; } = string.Empty;
    public StepStatus Status { get; set; }
    public DateTime? ValidatedAt { get; set; }
    public string? ValidatorId { get; set; }
    public string? ValidatorName { get; set; }
    public string? Comments { get; set; }
}

public class ApproveRejectStepDto
{
    public string? Comments { get; set; }
}
