using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Notifications.ListNotifications;

public class ListNotificationsHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<ListNotificationsQuery, ResultOf<NotificationListModel>>
{
    private const int MaxItems = 20;

    public async Task<ResultOf<NotificationListModel>> Handle(ListNotificationsQuery request, CancellationToken cancellationToken)
    {
        var unreadCount = await db.Notifications
            .CountAsync(x => x.UserId == user.Id && x.ReadAt == null, cancellationToken);

        var notifications = await db.Notifications
            .AsNoTracking()
            .Where(x => x.UserId == user.Id)
            .OrderByDescending(x => x.CreatedAt)
            .Take(MaxItems)
            .ToListAsync(cancellationToken);

        return ResultOf<NotificationListModel>.Success(new NotificationListModel
        {
            UnreadCount = unreadCount,
            Items = notifications.Select(NotificationModel.From).ToList()
        });
    }
}
