using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Models;

public class NotificationModel
{
    public long Id { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public long? ReferenceId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }

    public static NotificationModel From(Notification notification)
    {
        return new NotificationModel
        {
            Id = notification.Id,
            Type = notification.Type,
            Title = notification.Title,
            Message = notification.Message,
            ReferenceId = notification.ReferenceId,
            CreatedAt = notification.CreatedAt,
            IsRead = notification.IsRead
        };
    }
}

public class NotificationListModel
{
    public int UnreadCount { get; set; }
    public List<NotificationModel> Items { get; set; } = [];
}
