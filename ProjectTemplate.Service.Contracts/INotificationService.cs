using ProjectTemplate.Models.Entities;
using ProjectTemplate.Shared.RequestFeatures;
using ProjectTemplate.Shared.DataTransferObjects;

namespace ProjectTemplate.Service.Contracts;

public interface INotificationService : IBaseService<NotificationDto, Notification>
{
    Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId, RequestParameters parameters, bool trackChanges);
    Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(string userId, RequestParameters parameters, bool trackChanges);
    Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto);
    Task MarkAsReadAsync(Guid id, bool trackChanges);
    Task MarkAllAsReadAsync(string userId, bool trackChanges);
    Task<int> GetUnreadCountAsync(string userId, bool trackChanges);
}
