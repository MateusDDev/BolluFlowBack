using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Ingredients.UnitConversion.CreateConversion;
using BolluFlow.Application.Features.Ingredients.UnitConversion.DeleteConversion;
using BolluFlow.Application.Features.Ingredients.UnitConversion.ListConversions;
using BolluFlow.Application.Features.Ingredients.UnitConversion.UpdateConversion;
using BolluFlow.Application.Models;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers.Ingredients;

[ApiController]
[Route("ingredients/{ingredientId:long}/conversions")]
[Authorize]
public class IngredientConversionsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Result>> CreateIngredientConversion(
        [FromRoute] long ingredientId,
        [FromBody] CreateUnitCommandBody body,
        CancellationToken ct)
    {
        var command = new CreateConversionCommand(ingredientId, body);
        
        var result = await mediator.Send(command, ct);

        return result.ToActionResult();
    }

    [HttpDelete("{conversionId:long}")]
    public async Task<ActionResult<Result>> DeleteIngredientConversion(
        [FromRoute] long ingredientId,
        [FromRoute] long conversionId,
        CancellationToken ct)
    {
        var command = new DeleteConversionCommand(ingredientId, conversionId);
        
        var result = await mediator.Send(command, ct);

        return result.ToActionResult();
    }

    [HttpPut("{conversionId:long}")]
    public async Task<ActionResult<Result>> UpdateIngredientConversion(
        [FromRoute] long ingredientId,
        [FromRoute] long conversionId,
        [FromBody] UpdateUnitCommandBody body,
        CancellationToken ct)
    {
        var command = new UpdateConversionCommand(ingredientId, conversionId, body);
        
        var result = await mediator.Send(command, ct);

        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<ActionResult<ResultOf<List<IngredientConversionModel>>>> ListIngredientConversions(
        [FromRoute] long ingredientId)
    {
        var query = new ListConversionsQuery(ingredientId);
        
        var result = await mediator.Send(query);
        
        return result.ToActionResult();
    }
}