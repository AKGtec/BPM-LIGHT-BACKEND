using Microsoft.AspNetCore.Identity;
using ProjectTemplate.Contracts.Repository;
using ProjectTemplate.Models.Entities;

namespace ProjectTemplate.API.Extensions;

public static class DataSeeder
{
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var roles = new[] { "Admin", "Manager", "HR", "Director", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        const string adminEmail = "admin@bpm.com";
        const string adminPassword = "Admin123!";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }

    public static async Task SeedDefaultWorkflowsAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var repositoryManager = scope.ServiceProvider.GetRequiredService<IRepositoryManager>();

        // Check if workflows already exist
        var existingWorkflows = await repositoryManager.Workflow.GetAllWorkflowsAsync(new ProjectTemplate.Shared.RequestFeatures.EntityParameters(), false);
        if (existingWorkflows.Any())
        {
            return; // Workflows already exist, skip seeding
        }

        // Create Leave Request Workflow
        var leaveWorkflow = new Workflow
        {
            Name = "Leave Request Workflow",
            Description = "Standard workflow for leave requests",
            Version = 1,
            IsActive = true
        };

        leaveWorkflow.Steps.Add(new WorkflowStep
        {
            StepName = "Manager Approval",
            Order = 1,
            ResponsibleRole = "Manager",
            DueInHours = 24
        });

        leaveWorkflow.Steps.Add(new WorkflowStep
        {
            StepName = "HR Review",
            Order = 2,
            ResponsibleRole = "HR",
            DueInHours = 48
        });

        repositoryManager.Workflow.CreateWorkflow(leaveWorkflow);

        // Create Expense Request Workflow
        var expenseWorkflow = new Workflow
        {
            Name = "Expense Request Workflow",
            Description = "Standard workflow for expense requests",
            Version = 1,
            IsActive = true
        };

        expenseWorkflow.Steps.Add(new WorkflowStep
        {
            StepName = "Manager Approval",
            Order = 1,
            ResponsibleRole = "Manager",
            DueInHours = 24
        });

        expenseWorkflow.Steps.Add(new WorkflowStep
        {
            StepName = "Finance Review",
            Order = 2,
            ResponsibleRole = "Admin",
            DueInHours = 72
        });

        repositoryManager.Workflow.CreateWorkflow(expenseWorkflow);

        // Create Training Request Workflow
        var trainingWorkflow = new Workflow
        {
            Name = "Training Request Workflow",
            Description = "Standard workflow for training requests",
            Version = 1,
            IsActive = true
        };

        trainingWorkflow.Steps.Add(new WorkflowStep
        {
            StepName = "Manager Approval",
            Order = 1,
            ResponsibleRole = "Manager",
            DueInHours = 48
        });

        trainingWorkflow.Steps.Add(new WorkflowStep
        {
            StepName = "HR Approval",
            Order = 2,
            ResponsibleRole = "HR",
            DueInHours = 72
        });

        repositoryManager.Workflow.CreateWorkflow(trainingWorkflow);

        // Create IT Support Workflow
        var itWorkflow = new Workflow
        {
            Name = "IT Support Workflow",
            Description = "Standard workflow for IT support requests",
            Version = 1,
            IsActive = true
        };

        itWorkflow.Steps.Add(new WorkflowStep
        {
            StepName = "IT Team Review",
            Order = 1,
            ResponsibleRole = "Admin",
            DueInHours = 24
        });

        repositoryManager.Workflow.CreateWorkflow(itWorkflow);

        // Create Profile Update Workflow
        var profileWorkflow = new Workflow
        {
            Name = "Profile Update Workflow",
            Description = "Standard workflow for profile update requests",
            Version = 1,
            IsActive = true
        };

        profileWorkflow.Steps.Add(new WorkflowStep
        {
            StepName = "HR Approval",
            Order = 1,
            ResponsibleRole = "HR",
            DueInHours = 48
        });

        repositoryManager.Workflow.CreateWorkflow(profileWorkflow);

        await repositoryManager.SaveAsync();
    }
}
