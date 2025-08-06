using ProjectTemplate.Models.Entities;
using ProjectTemplate.Shared.RequestFeatures;

namespace ProjectTemplate.Contracts.Repository;

public interface IWorkflowRepository : IRepositoryBase<Workflow>
{
    Task<IEnumerable<Workflow>> GetAllWorkflowsAsync(RequestParameters parameters, bool trackChanges);
    Task<Workflow?> GetWorkflowAsync(Guid workflowId, bool trackChanges);
    Task<Workflow?> GetWorkflowWithStepsAsync(Guid workflowId, bool trackChanges);
    Task<IEnumerable<Workflow>> GetActiveWorkflowsAsync(RequestParameters parameters, bool trackChanges);
    void CreateWorkflow(Workflow workflow);
    void DeleteWorkflow(Workflow workflow);
}
