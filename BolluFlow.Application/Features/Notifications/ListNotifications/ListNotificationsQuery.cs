using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Notifications.ListNotifications;

public record ListNotificationsQuery : IQuery<ResultOf<NotificationListModel>>;
