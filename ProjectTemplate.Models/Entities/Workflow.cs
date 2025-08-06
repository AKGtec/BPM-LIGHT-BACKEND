using System.ComponentModel.DataAnnotations;

namespace ProjectTemplate.Models.Entities;

public class Workflow : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    public int Version { get; set; } = 1;

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<WorkflowStep> Steps { get; set; } = new List<WorkflowStep>();
}
