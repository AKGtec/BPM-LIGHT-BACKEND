using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectTemplate.Models.Entities;

public class LeaveBalance : BaseEntity
{
    [Required]
    [MaxLength(450)]
    public string EmployeeId { get; set; } = string.Empty;

    [Required]
    public LeaveType LeaveType { get; set; }

    [Required]
    public int Year { get; set; } = DateTime.UtcNow.Year;

    [Required]
    public int TotalDays { get; set; }

    [Required]
    public int UsedDays { get; set; } = 0;

    [Required]
    public int RemainingDays { get; set; }

    public int CarriedForwardDays { get; set; } = 0;

    public DateTime? LastUpdated { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey(nameof(EmployeeId))]
    public virtual IdentityUser Employee { get; set; } = null!;

    // Computed property
    [NotMapped]
    public int AvailableDays => RemainingDays + CarriedForwardDays;
}