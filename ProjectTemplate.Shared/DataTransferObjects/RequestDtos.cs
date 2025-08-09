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

public class RequestFilterDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public RequestType? Type { get; set; }
    public RequestStatus? Status { get; set; }
    public string? InitiatorId { get; set; }
    public string? Department { get; set; }
    public string? SearchTerm { get; set; }
    public List<string>? Tags { get; set; }
}

public class CancelRequestDto
{
    public string? Reason { get; set; }
}

public class BulkActionDto
{
    public List<Guid> RequestIds { get; set; } = new();
    public string? Comments { get; set; }
}

public class RequestSummaryDto
{
    public int TotalRequests { get; set; }
    public int PendingRequests { get; set; }
    public int ApprovedRequests { get; set; }
    public int RejectedRequests { get; set; }
    public int ArchivedRequests { get; set; }
    public Dictionary<RequestType, int> RequestsByType { get; set; } = new();
    public Dictionary<RequestStatus, int> RequestsByStatus { get; set; } = new();
}

public class RequestStatisticsDto
{
    public int TotalRequests { get; set; }
    public int PendingRequests { get; set; }
    public int ApprovedRequests { get; set; }
    public int RejectedRequests { get; set; }
    public int CancelledRequests { get; set; }
    public double AverageProcessingTime { get; set; }
    public Dictionary<RequestType, int> RequestsByType { get; set; } = new();
    public Dictionary<string, int> RequestsByMonth { get; set; } = new();
    public Dictionary<string, double> AverageProcessingTimeByType { get; set; } = new();
    public List<TopInitiatorDto> TopInitiators { get; set; } = new();
}

public class TopInitiatorDto
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public int RequestCount { get; set; }
}

public class RequestHistoryDto
{
    public Guid Id { get; set; }
    public Guid RequestId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Comments { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
}

public class BulkActionResultDto
{
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<Guid> ProcessedRequestIds { get; set; } = new();
}
