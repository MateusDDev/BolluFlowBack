using BolluFlow.Application.Features.Dashboard;
using BolluFlow.Application.Features.Dashboard.GetLowStockIngredients;
using BolluFlow.Application.Features.Dashboard.GetLowStockProducts;
using BolluFlow.Application.Features.Dashboard.GetLowStockSupplies;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers;

[ApiController]
[Route("dashboard")]
[Authorize]
public class DashboardController(IMediator mediator, IDashboardReadService dashboardReadService) : ControllerBase
{
    [HttpGet("stock-alerts")]
    [ProducesResponseType(typeof(IReadOnlyList<StockAlertResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<StockAlertResponse>>> GetStockAlerts([FromQuery] DashboardLimitQuery query, CancellationToken ct)
    {
        var response = await dashboardReadService.GetStockAlertsAsync(query.Limit ?? 10, ct);
        return Ok(response);
    }

    [HttpGet("orders-by-status")]
    [ProducesResponseType(typeof(IReadOnlyList<OrdersByStatusResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<OrdersByStatusResponse>>> GetOrdersByStatus(CancellationToken ct)
    {
        var response = await dashboardReadService.GetOrdersByStatusAsync(ct);
        return Ok(response);
    }

    [HttpGet("inventory-value-by-type")]
    [ProducesResponseType(typeof(IReadOnlyList<InventoryValueByTypeResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<InventoryValueByTypeResponse>>> GetInventoryValueByType(CancellationToken ct)
    {
        var response = await dashboardReadService.GetInventoryValueByTypeAsync(ct);
        return Ok(response);
    }

    [HttpGet("recent-orders")]
    [ProducesResponseType(typeof(IReadOnlyList<RecentOrderResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RecentOrderResponse>>> GetRecentOrders([FromQuery] DashboardRecentOrdersLimitQuery query, CancellationToken ct)
    {
        var response = await dashboardReadService.GetRecentOrdersAsync(query.Limit ?? 5, ct);
        return Ok(response);
    }

    [HttpGet("low-stock-ingredients")]
    public async Task<ActionResult<List<GetLowStockIngredientsResult>>> ListLowStockIngredients(CancellationToken ct)
    {
        var result = await mediator.Send(new GetLowStockIngredientsQuery(), ct);
        return result.ToActionResult();
    }

    [HttpGet("low-stock-supplies")]
    public async Task<ActionResult<List<GetLowStockSuppliesResult>>> ListLowStockSupplies(CancellationToken ct)
    {
        var result = await mediator.Send(new GetLowStockSuppliesQuery(), ct);
        return result.ToActionResult();
    }

    [HttpGet("low-stock-products")]
    public async Task<ActionResult<List<GetLowStockProductsResult>>> ListLowStockProducts(CancellationToken ct)
    {
        var result = await mediator.Send(new GetLowStockProductsQuery(), ct);
        return result.ToActionResult();
    }
}
