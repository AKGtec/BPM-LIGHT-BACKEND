namespace ProjectTemplate.Contracts.Repository;

public interface IRepositoryManager
{
    IWorkflowRepository Workflow { get; }
    IRequestRepository Request { get; }
    INotificationRepository Notification { get; }

    void Save();
    Task SaveAsync();
}
