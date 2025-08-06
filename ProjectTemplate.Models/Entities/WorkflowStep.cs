using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectTemplate.Models.Entities;

public class WorkflowStep : BaseEntity
{
    [Required]
    [ForeignKey(nameof(Workflow))]
    public Guid WorkflowId { get; set; }

    [Required]
    [MaxLength(200)]
    public string StepName { get; set; } = string.Empty;

    [Required]
    public int Order { get; set; }

    [Required]
    [MaxLength(100)]
    public string ResponsibleRole { get; set; } = string.Empty;

    public int? DueInHours { get; set; }

    // Navigation properties
    public virtual Workflow Workflow { get; set; } = null!;
    public virtual ICollection<RequestStep> RequestSteps { get; set; } = new List<RequestStep>();
}
