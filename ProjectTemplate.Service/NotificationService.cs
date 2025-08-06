using AutoMapper;
using ProjectTemplate.Contracts;
using ProjectTemplate.Contracts.Repository;
using ProjectTemplate.Models.Entities;
using ProjectTemplate.Service.Contracts;
using ProjectTemplate.Shared.DataTransferObjects;
using ProjectTemplate.Shared.RequestFeatures;

namespace ProjectTemplate.Service;

public class NotificationService : BaseService<NotificationDto, Notification>, INotificationService
{
    public NotificationService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        : base(repository, logger, mapper)
    {
    }

    public override async Task<IEnumerable<NotificationDto>> GetAllAsync(RequestParameters parameters, bool trackChanges)
    {
        var notifications = await _repository.Notification.GetAllNotificationsAsync(parameters, trackChanges);
        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public override async Task<NotificationDto?> GetByIdAsync(Guid id, bool trackChanges)
    {
        var notification = await _repository.Notification.GetNotificationAsync(id, trackChanges);
        return notification == null ? null : _mapper.Map<NotificationDto>(notification);
    }

    public override async Task<NotificationDto> CreateAsync(NotificationDto dto)
    {
        var notification = _mapper.Map<Notification>(dto);
        _repository.Notification.CreateNotification(notification);
        await _repository.SaveAsync();

        return _mapper.Map<NotificationDto>(notification);
    }

    public override async Task UpdateAsync(Guid id, NotificationDto dto, bool trackChanges)
    {
        var notification = await _repository.Notification.GetNotificationAsync(id, trackChanges);
        CheckIfEntityExists(notification, id);

        _mapper.Map(dto, notification);
        await _repository.SaveAsync();
    }

    public override async Task DeleteAsync(Guid id, bool trackChanges)
    {
        var notification = await _repository.Notification.GetNotificationAsync(id, trackChanges);
        CheckIfEntityExists(notification, id);

        _repository.Notification.DeleteNotification(notification!);
        await _repository.SaveAsync();
    }

    public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId, RequestParameters parameters, bool trackChanges)
    {
        var notifications = await _repository.Notification.GetUserNotificationsAsync(userId, parameters, trackChanges);
        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public async Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(string userId, RequestParameters parameters, bool trackChanges)
    {
        var notifications = await _repository.Notification.GetUnreadNotificationsAsync(userId, parameters, trackChanges);
        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto)
    {
        var notification = _mapper.Map<Notification>(dto);
        _repository.Notification.CreateNotification(notification);
        await _repository.SaveAsync();

        return _mapper.Map<NotificationDto>(notification);
    }

    public async Task MarkAsReadAsync(Guid id, bool trackChanges)
    {
        var notification = await _repository.Notification.GetNotificationAsync(id, trackChanges);
        CheckIfEntityExists(notification, id);

        notification!.IsRead = true;
        await _repository.SaveAsync();
    }

    public async Task MarkAllAsReadAsync(string userId, bool trackChanges)
    {
        var unreadNotifications = await _repository.Notification.GetUnreadNotificationsAsync(userId, new EntityParameters { PageSize = int.MaxValue }, trackChanges);
        
        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
        }

        await _repository.SaveAsync();
    }

    public async Task<int> GetUnreadCountAsync(string userId, bool trackChanges)
    {
        return await _repository.Notification.GetUnreadCountAsync(userId, trackChanges);
    }
}
