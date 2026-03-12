using BolluFlow.Application.Features.Dashboard.GetInventoryValueByType;
using BolluFlow.Application.Features.Dashboard.GetOrdersByStatus;
using BolluFlow.Application.Features.Dashboard.GetRecentOrders;
using BolluFlow.Application.Features.Dashboard.GetStockAlerts;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers;

[ApiController]
[Route("dashboard")]
[Authorize]
public class DashboardController(IMediator mediator) : ControllerBase
{
    [HttpGet("stock-alerts")]
    [ProducesResponseType(typeof(List<GetStockAlertsResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GetStockAlertsResult>>> GetStockAlerts([FromQuery] int? limit, CancellationToken ct)
    {
        var result = await mediator.Send(new GetStockAlertsQuery(limit), ct);
        return result.ToActionResult();
    }

    [HttpGet("orders-by-status")]
    [ProducesResponseType(typeof(List<GetOrdersByStatusResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GetOrdersByStatusResult>>> GetOrdersByStatus(CancellationToken ct)
    {
        var result = await mediator.Send(new GetOrdersByStatusQuery(), ct);
        return result.ToActionResult();
    }

    [HttpGet("inventory-value-by-type")]
    [ProducesResponseType(typeof(List<GetInventoryValueByTypeResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GetInventoryValueByTypeResult>>> GetInventoryValueByType(CancellationToken ct)
    {
        var result = await mediator.Send(new GetInventoryValueByTypeQuery(), ct);
        return result.ToActionResult();
    }

    [HttpGet("recent-orders")]
    [ProducesResponseType(typeof(List<GetRecentOrdersResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GetRecentOrdersResult>>> GetRecentOrders([FromQuery] int? limit, CancellationToken ct)
    {
        var result = await mediator.Send(new GetRecentOrdersQuery(limit), ct);
        return result.ToActionResult();
    }
}
