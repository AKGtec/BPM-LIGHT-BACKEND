using AutoMapper;
using ProjectTemplate.Contracts;
using ProjectTemplate.Contracts.Repository;
using ProjectTemplate.Models.Entities;
using ProjectTemplate.Service.Contracts;
using ProjectTemplate.Shared.DataTransferObjects;
using ProjectTemplate.Shared.RequestFeatures;

namespace ProjectTemplate.Service;

public class WorkflowService : BaseService<WorkflowDto, Workflow>, IWorkflowService
{
    public WorkflowService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        : base(repository, logger, mapper)
    {
    }

    public override async Task<IEnumerable<WorkflowDto>> GetAllAsync(RequestParameters parameters, bool trackChanges)
    {
        var workflows = await _repository.Workflow.GetAllWorkflowsAsync(parameters, trackChanges);
        return _mapper.Map<IEnumerable<WorkflowDto>>(workflows);
    }

    public override async Task<WorkflowDto?> GetByIdAsync(Guid id, bool trackChanges)
    {
        var workflow = await _repository.Workflow.GetWorkflowAsync(id, trackChanges);
        return workflow == null ? null : _mapper.Map<WorkflowDto>(workflow);
    }

    public override async Task<WorkflowDto> CreateAsync(WorkflowDto dto)
    {
        var workflow = _mapper.Map<Workflow>(dto);
        _repository.Workflow.CreateWorkflow(workflow);
        await _repository.SaveAsync();

        return _mapper.Map<WorkflowDto>(workflow);
    }

    public override async Task UpdateAsync(Guid id, WorkflowDto dto, bool trackChanges)
    {
        var workflow = await _repository.Workflow.GetWorkflowAsync(id, trackChanges);
        CheckIfEntityExists(workflow, id);

        _mapper.Map(dto, workflow);
        await _repository.SaveAsync();
    }

    public override async Task DeleteAsync(Guid id, bool trackChanges)
    {
        var workflow = await _repository.Workflow.GetWorkflowAsync(id, trackChanges);
        CheckIfEntityExists(workflow, id);

        _repository.Workflow.DeleteWorkflow(workflow!);
        await _repository.SaveAsync();
    }

    public async Task<IEnumerable<WorkflowDto>> GetActiveWorkflowsAsync(RequestParameters parameters, bool trackChanges)
    {
        var workflows = await _repository.Workflow.GetActiveWorkflowsAsync(parameters, trackChanges);
        return _mapper.Map<IEnumerable<WorkflowDto>>(workflows);
    }

    public async Task<WorkflowDto?> GetWorkflowWithStepsAsync(Guid id, bool trackChanges)
    {
        var workflow = await _repository.Workflow.GetWorkflowWithStepsAsync(id, trackChanges);
        return workflow == null ? null : _mapper.Map<WorkflowDto>(workflow);
    }

    public async Task<WorkflowDto> CreateWorkflowWithStepsAsync(CreateWorkflowDto dto)
    {
        var workflow = _mapper.Map<Workflow>(dto);
        
        // Ensure steps are ordered correctly
        for (int i = 0; i < workflow.Steps.Count; i++)
        {
            workflow.Steps.ElementAt(i).Order = i + 1;
        }

        _repository.Workflow.CreateWorkflow(workflow);
        await _repository.SaveAsync();

        return _mapper.Map<WorkflowDto>(workflow);
    }

    public async Task ActivateWorkflowAsync(Guid id, bool trackChanges)
    {
        var workflow = await _repository.Workflow.GetWorkflowAsync(id, trackChanges);
        CheckIfEntityExists(workflow, id);

        workflow!.IsActive = true;
        await _repository.SaveAsync();
    }

    public async Task DeactivateWorkflowAsync(Guid id, bool trackChanges)
    {
        var workflow = await _repository.Workflow.GetWorkflowAsync(id, trackChanges);
        CheckIfEntityExists(workflow, id);

        workflow!.IsActive = false;
        await _repository.SaveAsync();
    }
}
