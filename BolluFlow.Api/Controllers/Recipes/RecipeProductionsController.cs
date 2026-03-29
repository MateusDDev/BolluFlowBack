using BolluFlow.Application.Features.Recipes.Productions.RecordProduction;
using BolluFlow.Application.Models;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers.Recipes;

[ApiController]
[Route("recipes/{recipeId:long}/productions")]
[Authorize]
public class RecipeProductionsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<RecipeProductionModel>> Create(
        [FromRoute] long recipeId,
        [FromBody] RecordRecipeProductionCommandBody body,
        CancellationToken ct)
    {
        var result = await mediator.Send(new RecordRecipeProductionCommand(recipeId, body), ct);

        if (result.IsSuccessful)
            return StatusCode(201, result.Value);

        return result.ToActionResult();
    }
}
