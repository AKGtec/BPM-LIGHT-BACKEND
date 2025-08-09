namespace ProjectTemplate.Contracts.Repository;

public interface IRepositoryManager
{
    IWorkflowRepository Workflow { get; }
    IRequestRepository Request { get; }
    INotificationRepository Notification { get; }
    ILeaveRepository Leave { get; }
    ILeaveBalanceRepository LeaveBalance { get; }
    IPerformanceReviewRepository PerformanceReview { get; }

    void Save();
    Task SaveAsync();
}
