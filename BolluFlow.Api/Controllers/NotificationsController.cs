using BolluFlow.Application.Features.Notifications.ListNotifications;
using BolluFlow.Application.Features.Notifications.MarkAllNotificationsAsRead;
using BolluFlow.Application.Features.Notifications.MarkNotificationAsRead;
using BolluFlow.Application.Models;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers;

[ApiController]
[Authorize]
[Route("notifications")]
public class NotificationsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<NotificationListModel>> ListNotifications(CancellationToken ct)
    {
        var result = await mediator.Send(new ListNotificationsQuery(), ct);
        return result.ToActionResult();
    }

    [HttpPost("{id:long}/read")]
    public async Task<ActionResult> MarkAsRead([FromRoute] long id, CancellationToken ct)
    {
        var result = await mediator.Send(new MarkNotificationAsReadCommand(id), ct);
        return result.ToActionResult();
    }

    [HttpPost("read-all")]
    public async Task<ActionResult> MarkAllAsRead(CancellationToken ct)
    {
        var result = await mediator.Send(new MarkAllNotificationsAsReadCommand(), ct);
        return result.ToActionResult();
    }
}
