using BolluFlow.Application.Features.Recipes.UnitConversion.CreateConversion;
using BolluFlow.Application.Features.Recipes.UnitConversion.DeleteConversion;
using BolluFlow.Application.Features.Recipes.UnitConversion.ListConversions;
using BolluFlow.Application.Models;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers.Recipes;

[ApiController]
[Route("recipes/{recipeId:long}/conversions")]
[Authorize]
public class RecipeConversionsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<RecipeConversionModel>>> List([FromRoute] long recipeId, CancellationToken ct)
    {
        var result = await mediator.Send(new ListRecipeConversionsQuery(recipeId), ct);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromRoute] long recipeId, [FromBody] CreateRecipeUnitCommandBody body, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateRecipeConversionCommand(recipeId, body), ct);
        return result.ToActionResult();
    }

    [HttpDelete("{conversionId:long}")]
    public async Task<ActionResult> Delete([FromRoute] long recipeId, [FromRoute] long conversionId, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteRecipeConversionCommand(recipeId, conversionId), ct);
        return result.ToActionResult();
    }
}
