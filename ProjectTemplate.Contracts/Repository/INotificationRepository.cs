using ProjectTemplate.Models.Entities;
using ProjectTemplate.Shared.RequestFeatures;

namespace ProjectTemplate.Contracts.Repository;

public interface INotificationRepository : IRepositoryBase<Notification>
{
    Task<IEnumerable<Notification>> GetAllNotificationsAsync(RequestParameters parameters, bool trackChanges);
    Task<Notification?> GetNotificationAsync(Guid notificationId, bool trackChanges);
    Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId, RequestParameters parameters, bool trackChanges);
    Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId, RequestParameters parameters, bool trackChanges);
    Task<int> GetUnreadCountAsync(string userId, bool trackChanges);
    void CreateNotification(Notification notification);
    void DeleteNotification(Notification notification);
}
