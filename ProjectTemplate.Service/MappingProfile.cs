using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ProjectTemplate.Models.Entities;
using ProjectTemplate.Shared.DataTransferObjects;

namespace ProjectTemplate.Service;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Workflow mappings
        CreateMap<Workflow, WorkflowDto>()
            .ForMember(dest => dest.Steps, opt => opt.MapFrom(src => src.Steps.OrderBy(s => s.Order)));
        
        CreateMap<CreateWorkflowDto, Workflow>()
            .ForMember(dest => dest.Steps, opt => opt.MapFrom(src => src.Steps));
        
        CreateMap<UpdateWorkflowDto, Workflow>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.Steps, opt => opt.Ignore());

        CreateMap<WorkflowDto, Workflow>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // WorkflowStep mappings
        CreateMap<WorkflowStep, WorkflowStepDto>();
        CreateMap<CreateWorkflowStepDto, WorkflowStep>();
        CreateMap<UpdateWorkflowStepDto, WorkflowStep>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.WorkflowId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.Workflow, opt => opt.Ignore())
            .ForMember(dest => dest.RequestSteps, opt => opt.Ignore());

        // Request mappings
        CreateMap<Request, RequestDto>()
            .ForMember(dest => dest.InitiatorName, opt => opt.MapFrom(src => src.Initiator != null ? src.Initiator.UserName : string.Empty))
            .ForMember(dest => dest.RequestSteps, opt => opt.MapFrom(src => src.RequestSteps.OrderBy(rs => rs.WorkflowStep.Order)));
        
        CreateMap<CreateRequestDto, Request>()
            .ForMember(dest => dest.InitiatorId, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => RequestStatus.Pending))
            .ForMember(dest => dest.Initiator, opt => opt.Ignore())
            .ForMember(dest => dest.RequestSteps, opt => opt.Ignore());
        
        CreateMap<UpdateRequestDto, Request>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.InitiatorId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.Initiator, opt => opt.Ignore())
            .ForMember(dest => dest.RequestSteps, opt => opt.Ignore());

        CreateMap<RequestDto, Request>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Initiator, opt => opt.Ignore())
            .ForMember(dest => dest.RequestSteps, opt => opt.Ignore());

        // RequestStep mappings
        CreateMap<RequestStep, RequestStepDto>()
            .ForMember(dest => dest.WorkflowStepName, opt => opt.MapFrom(src => src.WorkflowStep != null ? src.WorkflowStep.StepName : string.Empty))
            .ForMember(dest => dest.ResponsibleRole, opt => opt.MapFrom(src => src.WorkflowStep != null ? src.WorkflowStep.ResponsibleRole : string.Empty))
            .ForMember(dest => dest.ValidatorName, opt => opt.MapFrom(src => src.Validator != null ? src.Validator.UserName : string.Empty));

        // Notification mappings
        CreateMap<Notification, NotificationDto>();
        CreateMap<CreateNotificationDto, Notification>();
        CreateMap<UpdateNotificationDto, Notification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());

        CreateMap<NotificationDto, Notification>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.User, opt => opt.Ignore());

        // Identity User mappings
        CreateMap<IdentityUser, UserDto>()
            .ForMember(dest => dest.Roles, opt => opt.Ignore()); // Roles are handled separately

        CreateMap<UserRegistrationDto, IdentityUser>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
            .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore());

        // Leave mappings
        CreateMap<Leave, LeaveRequestDto>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.UserName ?? string.Empty))
            .ForMember(dest => dest.EmployeeEmail, opt => opt.MapFrom(src => src.Employee.Email ?? string.Empty))
            .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => 
                DeserializeAttachments(src.AttachmentPaths)));

        CreateMap<CreateLeaveRequestDto, Leave>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => LeaveStatus.Pending))
            .ForMember(dest => dest.SubmittedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.Employee, opt => opt.Ignore())
            .ForMember(dest => dest.Approver, opt => opt.Ignore())
            .ForMember(dest => dest.Rejector, opt => opt.Ignore())
            .ForMember(dest => dest.AttachmentPaths, opt => opt.Ignore());

        // LeaveBalance mappings
        CreateMap<LeaveBalance, LeaveBalanceDto>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.UserName ?? string.Empty));

        CreateMap<UpdateLeaveBalanceDto, LeaveBalance>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.EmployeeId, opt => opt.Ignore())
            .ForMember(dest => dest.LeaveType, opt => opt.Ignore())
            .ForMember(dest => dest.Year, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.Employee, opt => opt.Ignore())
            .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => DateTime.UtcNow));

        // PerformanceReview mappings
        CreateMap<PerformanceReview, PerformanceReviewDto>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.UserName ?? string.Empty))
            .ForMember(dest => dest.EmployeePosition, opt => opt.MapFrom(src => "Employee")) // Default position, can be enhanced
            .ForMember(dest => dest.ReviewerName, opt => opt.MapFrom(src => src.Reviewer != null ? src.Reviewer.UserName : null))
            .ForMember(dest => dest.Goals, opt => opt.MapFrom(src => 
                DeserializeGoals(src.Goals)));

        CreateMap<CreatePerformanceReviewDto, PerformanceReview>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ReviewStatus.Draft))
            .ForMember(dest => dest.Progress, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.Employee, opt => opt.Ignore())
            .ForMember(dest => dest.Reviewer, opt => opt.Ignore())
            .ForMember(dest => dest.Goals, opt => opt.MapFrom(src => 
                SerializeGoals(src.Goals)));

        CreateMap<UpdatePerformanceReviewDto, PerformanceReview>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.EmployeeId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.Employee, opt => opt.Ignore())
            .ForMember(dest => dest.Reviewer, opt => opt.Ignore())
            .ForMember(dest => dest.Goals, opt => opt.MapFrom(src => 
                SerializeGoals(src.Goals)))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }

    private static List<string> DeserializeAttachments(string? attachmentPaths)
    {
        if (string.IsNullOrEmpty(attachmentPaths))
            return new List<string>();
        
        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<string>>(attachmentPaths) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    private static List<string> DeserializeGoals(string? goals)
    {
        if (string.IsNullOrEmpty(goals))
            return new List<string>();
        
        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<string>>(goals) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    private static string? SerializeGoals(List<string>? goals)
    {
        if (goals == null || !goals.Any())
            return null;
        
        try
        {
            return System.Text.Json.JsonSerializer.Serialize(goals);
        }
        catch
        {
            return null;
        }
    }
}
