using BolluFlow.Domain.Enums;

namespace BolluFlow.Domain.Entities;

public class Notification
{
    private Notification() { }

    public long Id { get; private set; }
    public long UserId { get; private set; }
    public NotificationType Type { get; private set; }
    public string Title { get; private set; } = null!;
    public string Message { get; private set; } = null!;
    public long? ReferenceId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ReadAt { get; private set; }

    public bool IsRead => ReadAt.HasValue;

    public static Notification Create(long userId, NotificationType type, string title, string message, long? referenceId = null)
    {
        return new Notification
        {
            UserId = userId,
            Type = type,
            Title = title,
            Message = message,
            ReferenceId = referenceId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkAsRead()
    {
        ReadAt ??= DateTime.UtcNow;
    }
}
