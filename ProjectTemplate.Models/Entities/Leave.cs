using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectTemplate.Models.Entities;

public class Leave : BaseEntity
{
    [Required]
    [MaxLength(450)]
    public string EmployeeId { get; set; } = string.Empty;

    [Required]
    public LeaveType LeaveType { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    public int Days { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Reason { get; set; } = string.Empty;

    [Required]
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;

    [Required]
    public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;

    [MaxLength(450)]
    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedDate { get; set; }

    [MaxLength(450)]
    public string? RejectedBy { get; set; }

    public DateTime? RejectedDate { get; set; }

    [MaxLength(1000)]
    public string? RejectionReason { get; set; }

    [MaxLength(1000)]
    public string? Comments { get; set; }

    [MaxLength(2000)]
    public string? AttachmentPaths { get; set; } // JSON array of file paths

    // Navigation properties
    [ForeignKey(nameof(EmployeeId))]
    public virtual IdentityUser Employee { get; set; } = null!;

    [ForeignKey(nameof(ApprovedBy))]
    public virtual IdentityUser? Approver { get; set; }

    [ForeignKey(nameof(RejectedBy))]
    public virtual IdentityUser? Rejector { get; set; }
}