using BolluFlow.Application.Abstractions;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Notifications.MarkNotificationAsRead;

public class MarkNotificationAsReadHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<MarkNotificationAsReadCommand, Result>
{
    public async Task<Result> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await db.Notifications
            .FirstOrDefaultAsync(x => x.Id == request.NotificationId && x.UserId == user.Id, cancellationToken);

        if (notification is null)
            return Result.NotFound(new DomainError("NOTIFICATION_NOT_FOUND", "Notificação não encontrada"));

        notification.MarkAsRead();

        await db.SaveChangesAsync(cancellationToken);

        return Result.NoContent();
    }
}
