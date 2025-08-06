using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTemplate.Service.Contracts;
using ProjectTemplate.Shared.DataTransferObjects;
using ProjectTemplate.Shared.RequestFeatures;

namespace ProjectTemplate.Presentation.Controllers;

public class WorkflowController : BaseApiController
{
    public WorkflowController(IServiceManager serviceManager) : base(serviceManager)
    {
    }

    /// <summary>
    /// Get all workflows with pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetWorkflows([FromQuery] RequestParameters parameters)
    {
        try
        {
            var workflows = await _serviceManager.WorkflowService.GetAllAsync(parameters, trackChanges: false);
            return Ok(workflows);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get active workflows only
    /// </summary>
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveWorkflows([FromQuery] RequestParameters parameters)
    {
        try
        {
            var workflows = await _serviceManager.WorkflowService.GetActiveWorkflowsAsync(parameters, trackChanges: false);
            return Ok(workflows);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get workflow by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetWorkflow(Guid id)
    {
        try
        {
            var workflow = await _serviceManager.WorkflowService.GetByIdAsync(id, trackChanges: false);
            if (workflow == null)
                return NotFound($"Workflow with ID {id} not found.");

            return Ok(workflow);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get workflow with its steps
    /// </summary>
    [HttpGet("{id:guid}/steps")]
    public async Task<IActionResult> GetWorkflowWithSteps(Guid id)
    {
        try
        {
            var workflow = await _serviceManager.WorkflowService.GetWorkflowWithStepsAsync(id, trackChanges: false);
            if (workflow == null)
                return NotFound($"Workflow with ID {id} not found.");

            return Ok(workflow);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Create a new workflow
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateWorkflow([FromBody] CreateWorkflowDto workflowDto)
    {
        try
        {
            if (workflowDto == null)
                return BadRequest("Workflow data is null.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var workflow = await _serviceManager.WorkflowService.CreateWorkflowWithStepsAsync(workflowDto);
            return CreatedAtAction(nameof(GetWorkflow), new { id = workflow.Id }, workflow);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Update an existing workflow
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateWorkflow(Guid id, [FromBody] UpdateWorkflowDto workflowDto)
    {
        try
        {
            if (workflowDto == null)
                return BadRequest("Workflow data is null.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var workflowToUpdate = _serviceManager.WorkflowService.GetByIdAsync(id, trackChanges: false);
            if (workflowToUpdate == null)
                return NotFound($"Workflow with ID {id} not found.");

            var mappedDto = new WorkflowDto
            {
                Id = id,
                Name = workflowDto.Name,
                Description = workflowDto.Description,
                Version = workflowDto.Version,
                IsActive = workflowDto.IsActive
            };

            await _serviceManager.WorkflowService.UpdateAsync(id, mappedDto, trackChanges: true);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Delete a workflow
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteWorkflow(Guid id)
    {
        try
        {
            var workflow = await _serviceManager.WorkflowService.GetByIdAsync(id, trackChanges: false);
            if (workflow == null)
                return NotFound($"Workflow with ID {id} not found.");

            await _serviceManager.WorkflowService.DeleteAsync(id, trackChanges: false);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Activate a workflow
    /// </summary>
    [HttpPatch("{id:guid}/activate")]
    public async Task<IActionResult> ActivateWorkflow(Guid id)
    {
        try
        {
            await _serviceManager.WorkflowService.ActivateWorkflowAsync(id, trackChanges: true);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Deactivate a workflow
    /// </summary>
    [HttpPatch("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateWorkflow(Guid id)
    {
        try
        {
            await _serviceManager.WorkflowService.DeactivateWorkflowAsync(id, trackChanges: true);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
