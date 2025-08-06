using ProjectTemplate.Models.Entities;
using ProjectTemplate.Shared.RequestFeatures;
using ProjectTemplate.Shared.DataTransferObjects;

namespace ProjectTemplate.Service.Contracts;

public interface IWorkflowService : IBaseService<WorkflowDto, Workflow>
{
    Task<IEnumerable<WorkflowDto>> GetActiveWorkflowsAsync(RequestParameters parameters, bool trackChanges);
    Task<WorkflowDto?> GetWorkflowWithStepsAsync(Guid id, bool trackChanges);
    Task<WorkflowDto> CreateWorkflowWithStepsAsync(CreateWorkflowDto dto);
    Task ActivateWorkflowAsync(Guid id, bool trackChanges);
    Task DeactivateWorkflowAsync(Guid id, bool trackChanges);
}
