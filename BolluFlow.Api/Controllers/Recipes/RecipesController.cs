using BolluFlow.Application.Features.Recipes.Create;
using BolluFlow.Application.Features.Recipes.DeleteRecipe;
using BolluFlow.Application.Features.Recipes.GetRecipeDetails;
using BolluFlow.Application.Features.Recipes.ListRecipes;
using BolluFlow.Application.Features.Recipes.ProduceRecipe;
using BolluFlow.Application.Features.Recipes.UpdateRecipe;
using BolluFlow.Application.Models;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers.Recipes;

[ApiController]
[Route("api/receitas")]
[Authorize]
public class RecipesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CreateRecipeResult>> CreateRecipe([FromBody] CreateRecipeCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpPatch("{recipeId:long}")]
    public async Task<ActionResult<RecipeModel>> UpdateRecipe([FromRoute] long recipeId, [FromBody] UpdateRecipeCommandBody body, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateRecipeCommand(recipeId, body), ct);
        return result.ToActionResult();
    }

    [HttpGet("{recipeId:long}")]
    public async Task<ActionResult<RecipeModel>> GetRecipeById([FromRoute] long recipeId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetRecipeDetailsQuery(recipeId), ct);
        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<ActionResult<List<RecipeModel>>> ListRecipes(CancellationToken ct)
    {
        var result = await mediator.Send(new ListRecipesQuery(), ct);
        return result.ToActionResult();
    }

    [HttpPost("{recipeId:long}/producao")]
    public async Task<ActionResult<RecipeModel>> ProduceRecipe([FromRoute] long recipeId, [FromBody] ProduceRecipeBody body, CancellationToken ct)
    {
        var result = await mediator.Send(new ProduceRecipeCommand(recipeId, body.ConversionId, body.Quantity, body.ProductionDate), ct);
        return result.ToActionResult();
    }

    [HttpDelete("{recipeId:long}")]
    public async Task<ActionResult> DeleteRecipe([FromRoute] long recipeId, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteRecipeCommand(recipeId), ct);
        return result.ToActionResult();
    }
}

public record ProduceRecipeBody(long? ConversionId, decimal Quantity, DateOnly ProductionDate);
