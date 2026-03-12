using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Supplies.UnitConversion.CreateConversion;
using BolluFlow.Application.Features.Supplies.UnitConversion.DeleteConversion;
using BolluFlow.Application.Features.Supplies.UnitConversion.ListConversions;
using BolluFlow.Application.Features.Supplies.UnitConversion.UpdateConversion;
using BolluFlow.Application.Models;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers.Supplies;

[ApiController]
[Route("supplies/{supplyId:long}/conversions")]
[Authorize]
public class SupplyConversionsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Result>> CreateSupplyConversion(
        [FromRoute] long supplyId,
        [FromBody] CreateSupplyUnitCommandBody body,
        CancellationToken ct)
    {
        var result = await mediator.Send(new CreateSupplyConversionCommand(supplyId, body), ct);
        return result.ToActionResult();
    }

    [HttpDelete("{conversionId:long}")]
    public async Task<ActionResult<Result>> DeleteSupplyConversion(
        [FromRoute] long supplyId,
        [FromRoute] long conversionId,
        CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteSupplyConversionCommand(supplyId, conversionId), ct);
        return result.ToActionResult();
    }

    [HttpPut("{conversionId:long}")]
    public async Task<ActionResult<Result>> UpdateSupplyConversion(
        [FromRoute] long supplyId,
        [FromRoute] long conversionId,
        [FromBody] UpdateSupplyUnitCommandBody body,
        CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateSupplyConversionCommand(supplyId, conversionId, body), ct);
        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<ActionResult<ResultOf<List<SupplyConversionModel>>>> ListSupplyConversions([FromRoute] long supplyId)
    {
        var result = await mediator.Send(new ListSupplyConversionsQuery(supplyId));
        return result.ToActionResult();
    }
}
