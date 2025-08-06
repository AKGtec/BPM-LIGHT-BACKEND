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
    }
}
