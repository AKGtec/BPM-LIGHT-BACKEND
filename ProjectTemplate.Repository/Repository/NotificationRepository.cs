using Microsoft.EntityFrameworkCore;
using ProjectTemplate.Contracts.Repository;
using ProjectTemplate.Models.DataSource;
using ProjectTemplate.Models.Entities;
using ProjectTemplate.Shared.RequestFeatures;

namespace ProjectTemplate.Repository.Repository;

public class NotificationRepository : RepositoryBase<Notification>, INotificationRepository
{
    public NotificationRepository(ProjectTemplateContext repositoryContext) : base(repositoryContext)
    {
    }

    public async Task<IEnumerable<Notification>> GetAllNotificationsAsync(RequestParameters parameters, bool trackChanges)
    {
        return await FindAll(trackChanges)
            .Include(n => n.User)
            .OrderByDescending(n => n.CreatedAt)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();
    }

    public async Task<Notification?> GetNotificationAsync(Guid notificationId, bool trackChanges)
    {
        return await FindByCondition(n => n.Id.Equals(notificationId), trackChanges)
            .Include(n => n.User)
            .SingleOrDefaultAsync();
    }

    public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId, RequestParameters parameters, bool trackChanges)
    {
        return await FindByCondition(n => n.UserId.Equals(userId), trackChanges)
            .OrderByDescending(n => n.CreatedAt)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId, RequestParameters parameters, bool trackChanges)
    {
        return await FindByCondition(n => n.UserId.Equals(userId) && !n.IsRead, trackChanges)
            .OrderByDescending(n => n.CreatedAt)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(string userId, bool trackChanges)
    {
        return await FindByCondition(n => n.UserId.Equals(userId) && !n.IsRead, trackChanges)
            .CountAsync();
    }

    public void CreateNotification(Notification notification) => Create(notification);

    public void DeleteNotification(Notification notification) => Delete(notification);
}
