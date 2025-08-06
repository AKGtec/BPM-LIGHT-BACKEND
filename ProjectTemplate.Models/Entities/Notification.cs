using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ProjectTemplate.Models.Entities;

public class Notification : BaseEntity
{
    [Required]
    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; } = false;

    [MaxLength(100)]
    public string? Type { get; set; }

    [MaxLength(500)]
    public string? ActionUrl { get; set; }

    // Navigation properties
    public virtual IdentityUser User { get; set; } = null!;
}
