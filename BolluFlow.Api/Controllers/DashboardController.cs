using BolluFlow.Application.Features.Dashboard.GetProductMargins;
using BolluFlow.Application.Features.Dashboard.GetSalesSummary;
using BolluFlow.Application.Features.Dashboard.GetStockAlerts;
using BolluFlow.Application.Features.Dashboard.GetToday;
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
    public async Task<ActionResult<List<GetStockAlertsResult>>> GetStockAlerts([FromQuery] int? limit, CancellationToken ct)
    {
        var result = await mediator.Send(new GetStockAlertsQuery(limit), ct);
        return result.ToActionResult();
    }

    [HttpGet("sales")]
    public async Task<ActionResult<GetSalesSummaryResult>> GetSalesSummary(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] int utcOffsetMinutes,
        [FromQuery] DateTime? previousFrom,
        CancellationToken ct)
    {
        var result = await mediator.Send(new GetSalesSummaryQuery(from, to, utcOffsetMinutes, previousFrom), ct);
        return result.ToActionResult();
    }

    [HttpGet("today")]
    public async Task<ActionResult<GetTodayResult>> GetToday([FromQuery] DateTime dayStart, CancellationToken ct)
    {
        var result = await mediator.Send(new GetTodayQuery(dayStart), ct);
        return result.ToActionResult();
    }

    [HttpGet("product-margins")]
    public async Task<ActionResult<List<GetProductMarginsResult>>> GetProductMargins(CancellationToken ct)
    {
        var result = await mediator.Send(new GetProductMarginsQuery(), ct);
        return result.ToActionResult();
    }
}
