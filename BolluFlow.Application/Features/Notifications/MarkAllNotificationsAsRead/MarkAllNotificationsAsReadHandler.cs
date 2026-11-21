using BolluFlow.Application.Abstractions;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Notifications.MarkAllNotificationsAsRead;

public class MarkAllNotificationsAsReadHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<MarkAllNotificationsAsReadCommand, Result>
{
    public async Task<Result> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
    {
        var readAt = DateTime.UtcNow;

        await db.Notifications
            .Where(x => x.UserId == user.Id && x.ReadAt == null)
            .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.ReadAt, readAt), cancellationToken);

        return Result.NoContent();
    }
}
