using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Notifications.MarkNotificationAsRead;

public record MarkNotificationAsReadCommand(long NotificationId) : ICommand<Result>;
