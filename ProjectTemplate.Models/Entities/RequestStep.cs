using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ProjectTemplate.Models.Entities;

public class RequestStep : BaseEntity
{
    [Required]
    [ForeignKey(nameof(Request))]
    public Guid RequestId { get; set; }

    [Required]
    [ForeignKey(nameof(WorkflowStep))]
    public Guid WorkflowStepId { get; set; }

    [Required]
    public StepStatus Status { get; set; } = StepStatus.Pending;

    public DateTime? ValidatedAt { get; set; }

    [ForeignKey(nameof(Validator))]
    public string? ValidatorId { get; set; }

    [MaxLength(1000)]
    public string? Comments { get; set; }

    // Navigation properties
    public virtual Request Request { get; set; } = null!;
    public virtual WorkflowStep WorkflowStep { get; set; } = null!;
    public virtual IdentityUser? Validator { get; set; }
}
