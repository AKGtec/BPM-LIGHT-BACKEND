using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ProjectTemplate.Models.Entities;

public class Request : BaseEntity
{
    [Required]
    public RequestType Type { get; set; }

    [Required]
    [ForeignKey(nameof(Initiator))]
    public string InitiatorId { get; set; } = string.Empty;

    [Required]
    public RequestStatus Status { get; set; } = RequestStatus.Pending;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(500)]
    public string? Title { get; set; }

    // Navigation properties
    public virtual IdentityUser Initiator { get; set; } = null!;
    public virtual ICollection<RequestStep> RequestSteps { get; set; } = new List<RequestStep>();
}
