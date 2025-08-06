using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using ProjectTemplate.Contracts;
using ProjectTemplate.Contracts.Repository;
using ProjectTemplate.Service.Contracts;

namespace ProjectTemplate.Service;

public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<IWorkflowService> _workflowService;
    private readonly Lazy<IRequestService> _requestService;
    private readonly Lazy<INotificationService> _notificationService;
    private readonly Lazy<IAuthenticationService> _authenticationService;

    public ServiceManager(
        IRepositoryManager repositoryManager,
        ILoggerManager logger,
        IMapper mapper,
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        _workflowService = new Lazy<IWorkflowService>(() => new WorkflowService(repositoryManager, logger, mapper));
        _requestService = new Lazy<IRequestService>(() => new RequestService(repositoryManager, logger, mapper));
        _notificationService = new Lazy<INotificationService>(() => new NotificationService(repositoryManager, logger, mapper));
        _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(userManager, signInManager, roleManager, configuration, logger, mapper));
    }

    public IWorkflowService WorkflowService => _workflowService.Value;
    public IRequestService RequestService => _requestService.Value;
    public INotificationService NotificationService => _notificationService.Value;
    public IAuthenticationService AuthenticationService => _authenticationService.Value;
}
