using ProjectTemplate.Contracts.Repository;
using ProjectTemplate.Models.DataSource;

namespace ProjectTemplate.Repository.Repository;

public sealed class RepositoryManager : IRepositoryManager
{
    private readonly ProjectTemplateContext _repositoryContext;
    
    private readonly Lazy<IWorkflowRepository> _workflowRepository;
    private readonly Lazy<IRequestRepository> _requestRepository;
    private readonly Lazy<INotificationRepository> _notificationRepository;

    public RepositoryManager(ProjectTemplateContext repositoryContext)
    {
        _repositoryContext = repositoryContext;

        _workflowRepository = new Lazy<IWorkflowRepository>(() => new WorkflowRepository(repositoryContext));
        _requestRepository = new Lazy<IRequestRepository>(() => new RequestRepository(repositoryContext));
        _notificationRepository = new Lazy<INotificationRepository>(() => new NotificationRepository(repositoryContext));
    }

    public IWorkflowRepository Workflow => _workflowRepository.Value;
    public IRequestRepository Request => _requestRepository.Value;
    public INotificationRepository Notification => _notificationRepository.Value;

    public void Save() => _repositoryContext.SaveChanges();

    public async Task SaveAsync() => await _repositoryContext.SaveChangesAsync();
}
