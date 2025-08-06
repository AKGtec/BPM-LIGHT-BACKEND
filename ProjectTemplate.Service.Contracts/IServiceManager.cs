namespace ProjectTemplate.Service.Contracts;

public interface IServiceManager
{
    IWorkflowService WorkflowService { get; }
    IRequestService RequestService { get; }
    INotificationService NotificationService { get; }
    IAuthenticationService AuthenticationService { get; }
}
