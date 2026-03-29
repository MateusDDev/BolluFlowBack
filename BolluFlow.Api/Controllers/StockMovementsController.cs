using BolluFlow.Application.Features.StockMovements.AdjustStockMovement;
using BolluFlow.Application.Features.StockMovements.CreateStockMovement;
using BolluFlow.Application.Features.StockMovements.ListStockMovements;
using BolluFlow.Application.Models;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Api.Controllers;

[ApiController]
[Route("stock/movements")]
[Authorize]
public class StockMovementsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<StockMovementModel>> Create(
        [FromBody] CreateStockMovementCommand command,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        if (result.IsSuccessful)
            return StatusCode(201, result.Value);

        return result.ToActionResult();
    }


    [HttpPost("adjustment")]
    public async Task<ActionResult<StockAdjustmentModel>> Adjust(
        [FromBody] AdjustStockMovementCommand command,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        if (result.IsSuccessful)
            return StatusCode(201, result.Value);

        return result.ToActionResult();
    }
    [HttpGet]
    public async Task<ActionResult<List<StockMovementModel>>> List(
        [FromQuery] string? itemType,
        [FromQuery] long? itemId,
        [FromQuery] string? movementType,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        CancellationToken ct)
    {
        var query = new ListStockMovementsQuery(itemType, itemId, movementType, startDate, endDate);
        var result = await mediator.Send(query, ct);
        return result.ToActionResult();
    }
}
