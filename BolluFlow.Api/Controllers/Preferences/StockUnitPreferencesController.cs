using BolluFlow.Application.Features.Preferences.StockUnit.GetStockUnitPreference;
using BolluFlow.Application.Features.Preferences.StockUnit.SaveStockUnitPreference;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Enums;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Api.Controllers.Preferences;

[ApiController]
[Route("preferences/stock-unit")]
[Authorize]
public class StockUnitPreferencesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<StockUnitPreferenceModel>> Get([FromQuery] StockItemType itemType, [FromQuery] long itemId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetStockUnitPreferenceQuery(itemType, itemId), ct);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<ActionResult<StockUnitPreferenceModel>> Save([FromBody] SaveStockUnitPreferenceBody body, CancellationToken ct)
    {
        var result = await mediator.Send(new SaveStockUnitPreferenceCommand(body), ct);

        if (result.IsSuccessful)
            return StatusCode(201, result.Value);

        return result.ToActionResult();
    }
}
