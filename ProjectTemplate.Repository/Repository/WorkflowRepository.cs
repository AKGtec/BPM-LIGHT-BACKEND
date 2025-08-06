using Microsoft.EntityFrameworkCore;
using ProjectTemplate.Contracts.Repository;
using ProjectTemplate.Models.DataSource;
using ProjectTemplate.Models.Entities;
using ProjectTemplate.Shared.RequestFeatures;

namespace ProjectTemplate.Repository.Repository;

public class WorkflowRepository : RepositoryBase<Workflow>, IWorkflowRepository
{
    public WorkflowRepository(ProjectTemplateContext repositoryContext) : base(repositoryContext)
    {
    }

    public async Task<IEnumerable<Workflow>> GetAllWorkflowsAsync(RequestParameters parameters, bool trackChanges)
    {
        return await FindAll(trackChanges)
            .OrderBy(w => w.Name)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();
    }

    public async Task<Workflow?> GetWorkflowAsync(Guid workflowId, bool trackChanges)
    {
        return await FindByCondition(w => w.Id.Equals(workflowId), trackChanges)
            .SingleOrDefaultAsync();
    }

    public async Task<Workflow?> GetWorkflowWithStepsAsync(Guid workflowId, bool trackChanges)
    {
        return await FindByCondition(w => w.Id.Equals(workflowId), trackChanges)
            .Include(w => w.Steps.OrderBy(s => s.Order))
            .SingleOrDefaultAsync();
    }

    public async Task<IEnumerable<Workflow>> GetActiveWorkflowsAsync(RequestParameters parameters, bool trackChanges)
    {
        return await FindByCondition(w => w.IsActive, trackChanges)
            .OrderBy(w => w.Name)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();
    }

    public void CreateWorkflow(Workflow workflow) => Create(workflow);

    public void DeleteWorkflow(Workflow workflow) => Delete(workflow);
}
