using BolluFlow.Application.Features.Recipes.AdjustStock;
using BolluFlow.Application.Models;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers.Recipes;

[ApiController]
[Route("recipes/{recipeId:long}/stock")]
[Authorize]
public class RecipeStockController(IMediator mediator) : ControllerBase
{
    [HttpPost("adjustment")]
    public async Task<ActionResult<RecipeStockAdjustmentModel>> Adjust(
        [FromRoute] long recipeId,
        [FromBody] AdjustRecipeStockCommandBody body,
        CancellationToken ct)
    {
        var result = await mediator.Send(new AdjustRecipeStockCommand(recipeId, body), ct);

        if (result.IsSuccessful)
            return StatusCode(201, result.Value);

        return result.ToActionResult();
    }
}
