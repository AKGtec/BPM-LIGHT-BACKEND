using ProjectTemplate.Models.Entities;

namespace ProjectTemplate.Shared.DataTransferObjects;

public class WorkflowDto : BaseDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Version { get; set; }
    public bool IsActive { get; set; }
    public ICollection<WorkflowStepDto> Steps { get; set; } = new List<WorkflowStepDto>();
}

public class CreateWorkflowDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Version { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    public ICollection<CreateWorkflowStepDto> Steps { get; set; } = new List<CreateWorkflowStepDto>();
}

public class UpdateWorkflowDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Version { get; set; }
    public bool IsActive { get; set; }
}

public class WorkflowStepDto : BaseDto
{
    public Guid WorkflowId { get; set; }
    public string StepName { get; set; } = string.Empty;
    public int Order { get; set; }
    public string ResponsibleRole { get; set; } = string.Empty;
    public int? DueInHours { get; set; }
}

public class CreateWorkflowStepDto
{
    public string StepName { get; set; } = string.Empty;
    public int Order { get; set; }
    public string ResponsibleRole { get; set; } = string.Empty;
    public int? DueInHours { get; set; }
}

public class UpdateWorkflowStepDto
{
    public string StepName { get; set; } = string.Empty;
    public int Order { get; set; }
    public string ResponsibleRole { get; set; } = string.Empty;
    public int? DueInHours { get; set; }
}
