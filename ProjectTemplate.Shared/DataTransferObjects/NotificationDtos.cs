namespace ProjectTemplate.Shared.DataTransferObjects;

public class NotificationDto : BaseDto
{
    public string UserId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public string? Type { get; set; }
    public string? ActionUrl { get; set; }
}

public class CreateNotificationDto
{
    public string UserId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Type { get; set; }
    public string? ActionUrl { get; set; }
}

public class UpdateNotificationDto
{
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public string? Type { get; set; }
    public string? ActionUrl { get; set; }
}
