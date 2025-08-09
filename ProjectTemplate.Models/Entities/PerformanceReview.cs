using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectTemplate.Models.Entities;

public class PerformanceReview : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(450)]
    public string EmployeeId { get; set; } = string.Empty;

    [Required]
    public ReviewType ReviewType { get; set; }

    [Required]
    public ReviewStatus Status { get; set; } = ReviewStatus.Draft;

    [Required]
    public DateTime DueDate { get; set; }

    [Range(0, 100)]
    public int Progress { get; set; } = 0;

    [Required]
    public ReviewPriority Priority { get; set; } = ReviewPriority.Medium;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(4000)]
    public string? Goals { get; set; } // JSON array of goals

    [MaxLength(4000)]
    public string? Feedback { get; set; }

    [Range(0, 5)]
    [Column(TypeName = "decimal(3,2)")]
    public decimal? Rating { get; set; }

    [MaxLength(450)]
    public string? ReviewerId { get; set; }

    public DateTime? CompletedDate { get; set; }

    public DateTime? StartedDate { get; set; }

    [MaxLength(1000)]
    public string? CancellationReason { get; set; }

    // Navigation properties
    [ForeignKey(nameof(EmployeeId))]
    public virtual IdentityUser Employee { get; set; } = null!;

    [ForeignKey(nameof(ReviewerId))]
    public virtual IdentityUser? Reviewer { get; set; }
}