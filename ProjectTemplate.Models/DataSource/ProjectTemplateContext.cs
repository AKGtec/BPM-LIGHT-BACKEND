using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectTemplate.Models.Entities;

namespace ProjectTemplate.Models.DataSource;

public class ProjectTemplateContext : IdentityDbContext
{
    public ProjectTemplateContext(DbContextOptions<ProjectTemplateContext> options) : base(options)
    {
    }

    // Business Process Automation DbSets
    public DbSet<Workflow> Workflows { get; set; }
    public DbSet<WorkflowStep> WorkflowSteps { get; set; }
    public DbSet<Request> Requests { get; set; }
    public DbSet<RequestStep> RequestSteps { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    // HR Management DbSets
    public DbSet<Leave> Leaves { get; set; }
    public DbSet<LeaveBalance> LeaveBalances { get; set; }
    public DbSet<PerformanceReview> PerformanceReviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Workflow relationships
        modelBuilder.Entity<Workflow>()
            .HasMany(w => w.Steps)
            .WithOne(ws => ws.Workflow)
            .HasForeignKey(ws => ws.WorkflowId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure WorkflowStep relationships
        modelBuilder.Entity<WorkflowStep>()
            .HasMany(ws => ws.RequestSteps)
            .WithOne(rs => rs.WorkflowStep)
            .HasForeignKey(rs => rs.WorkflowStepId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Request relationships
        modelBuilder.Entity<Request>()
            .HasOne(r => r.Initiator)
            .WithMany()
            .HasForeignKey(r => r.InitiatorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Request>()
            .HasMany(r => r.RequestSteps)
            .WithOne(rs => rs.Request)
            .HasForeignKey(rs => rs.RequestId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure RequestStep relationships
        modelBuilder.Entity<RequestStep>()
            .HasOne(rs => rs.Validator)
            .WithMany()
            .HasForeignKey(rs => rs.ValidatorId)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure Notification relationships
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure indexes for better performance
        modelBuilder.Entity<Request>()
            .HasIndex(r => r.Status);

        modelBuilder.Entity<Request>()
            .HasIndex(r => r.InitiatorId);

        modelBuilder.Entity<RequestStep>()
            .HasIndex(rs => rs.Status);

        modelBuilder.Entity<Notification>()
            .HasIndex(n => new { n.UserId, n.IsRead });

        modelBuilder.Entity<WorkflowStep>()
            .HasIndex(ws => new { ws.WorkflowId, ws.Order })
            .IsUnique();

        // Configure Leave relationships
        modelBuilder.Entity<Leave>()
            .HasOne(l => l.Employee)
            .WithMany()
            .HasForeignKey(l => l.EmployeeId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Leave>()
            .HasOne(l => l.Approver)
            .WithMany()
            .HasForeignKey(l => l.ApprovedBy)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Leave>()
            .HasOne(l => l.Rejector)
            .WithMany()
            .HasForeignKey(l => l.RejectedBy)
            .OnDelete(DeleteBehavior.NoAction);

        // Configure LeaveBalance relationships
        modelBuilder.Entity<LeaveBalance>()
            .HasOne(lb => lb.Employee)
            .WithMany()
            .HasForeignKey(lb => lb.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure PerformanceReview relationships
        modelBuilder.Entity<PerformanceReview>()
            .HasOne(pr => pr.Employee)
            .WithMany()
            .HasForeignKey(pr => pr.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PerformanceReview>()
            .HasOne(pr => pr.Reviewer)
            .WithMany()
            .HasForeignKey(pr => pr.ReviewerId)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure indexes for HR entities
        modelBuilder.Entity<Leave>()
            .HasIndex(l => l.EmployeeId);

        modelBuilder.Entity<Leave>()
            .HasIndex(l => l.Status);

        modelBuilder.Entity<Leave>()
            .HasIndex(l => new { l.StartDate, l.EndDate });

        modelBuilder.Entity<LeaveBalance>()
            .HasIndex(lb => new { lb.EmployeeId, lb.LeaveType, lb.Year })
            .IsUnique();

        modelBuilder.Entity<PerformanceReview>()
            .HasIndex(pr => pr.EmployeeId);

        modelBuilder.Entity<PerformanceReview>()
            .HasIndex(pr => pr.Status);

        modelBuilder.Entity<PerformanceReview>()
            .HasIndex(pr => pr.DueDate);

        modelBuilder.Entity<PerformanceReview>()
            .HasIndex(pr => pr.ReviewerId);
    }
}
