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
        var query = FindAll(trackChanges);

        // Apply sorting
        if (!string.IsNullOrEmpty(parameters.SortBy))
        {
            switch (parameters.SortBy.ToLower())
            {
                case "name":
                    query = parameters.SortDirection?.ToLower() == "desc"
                        ? query.OrderByDescending(w => w.Name)
                        : query.OrderBy(w => w.Name);
                    break;
                case "createdat":
                    query = parameters.SortDirection?.ToLower() == "desc"
                        ? query.OrderByDescending(w => w.CreatedAt)
                        : query.OrderBy(w => w.CreatedAt);
                    break;
                case "updatedat":
                    query = parameters.SortDirection?.ToLower() == "desc"
                        ? query.OrderByDescending(w => w.UpdatedAt)
                        : query.OrderBy(w => w.UpdatedAt);
                    break;
                case "version":
                    query = parameters.SortDirection?.ToLower() == "desc"
                        ? query.OrderByDescending(w => w.Version)
                        : query.OrderBy(w => w.Version);
                    break;
                case "isactive":
                    query = parameters.SortDirection?.ToLower() == "desc"
                        ? query.OrderByDescending(w => w.IsActive)
                        : query.OrderBy(w => w.IsActive);
                    break;
                default:
                    query = query.OrderBy(w => w.Name); // Default sorting
                    break;
            }
        }
        else
        {
            query = query.OrderBy(w => w.Name); // Default sorting
        }

        return await query
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
        var query = FindByCondition(w => w.IsActive, trackChanges);

        // Apply sorting
        if (!string.IsNullOrEmpty(parameters.SortBy))
        {
            switch (parameters.SortBy.ToLower())
            {
                case "name":
                    query = parameters.SortDirection?.ToLower() == "desc"
                        ? query.OrderByDescending(w => w.Name)
                        : query.OrderBy(w => w.Name);
                    break;
                case "createdat":
                    query = parameters.SortDirection?.ToLower() == "desc"
                        ? query.OrderByDescending(w => w.CreatedAt)
                        : query.OrderBy(w => w.CreatedAt);
                    break;
                case "updatedat":
                    query = parameters.SortDirection?.ToLower() == "desc"
                        ? query.OrderByDescending(w => w.UpdatedAt)
                        : query.OrderBy(w => w.UpdatedAt);
                    break;
                case "version":
                    query = parameters.SortDirection?.ToLower() == "desc"
                        ? query.OrderByDescending(w => w.Version)
                        : query.OrderBy(w => w.Version);
                    break;
                default:
                    query = query.OrderBy(w => w.Name); // Default sorting
                    break;
            }
        }
        else
        {
            query = query.OrderBy(w => w.Name); // Default sorting
        }

        return await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();
    }

    public void CreateWorkflow(Workflow workflow) => Create(workflow);

    public void DeleteWorkflow(Workflow workflow) => Delete(workflow);

    public async Task<int> GetWorkflowCountAsync()
    {
        return await FindAll(false).CountAsync();
    }
}
