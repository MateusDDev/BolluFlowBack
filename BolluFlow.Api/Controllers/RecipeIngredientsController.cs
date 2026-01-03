using BolluFlow.Application.Features.Recipes.RecipeIngredients.CreateRecipeIngredient;
using BolluFlow.Application.Features.Recipes.RecipeIngredients.DeleteRecipeIngredient;
using BolluFlow.Application.Features.Recipes.RecipeIngredients.GetRecipeIngredients;
using BolluFlow.Application.Features.Recipes.RecipeIngredients.UpdateRecipeIngredient;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers;

[ApiController]
[Route("recipes/{recipeId:long}/ingredients")]
[Authorize]
public class RecipeIngredientsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetRecipeIngredients([FromRoute] long recipeId, CancellationToken ct)
    {
        var query = new GetRecipeIngredientsQuery(recipeId);
        
        var result = await mediator.Send(query, ct);
        
        return result.ToActionResult();
    }

    [HttpDelete("{ingredientId:long}")]
    public async Task<IActionResult> DeleteRecipeIngredients(
        [FromRoute] long recipeId,
        [FromRoute] long ingredientId, 
        CancellationToken ct)
    {
        var command = new DeleteRecipeIngredientCommand(recipeId, ingredientId);
        
        var result = await mediator.Send(command, ct);
        
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> CreateRecipeIngredients(
        [FromRoute] long recipeId,
        [FromBody] CreateRecipeIngredientCommandBody body, 
        CancellationToken ct)
    {
        var command = new CreateRecipeIngredientCommand(recipeId, body);
        
        var result = await mediator.Send(command, ct);
        
        return result.ToActionResult();
    }

    [HttpPut("{ingredientId:long}")]
    public async Task<IActionResult> UpdateRecipeIngredients(
        [FromRoute] long recipeId,
        [FromRoute] long ingredientId,
        [FromBody] UpdateRecipeIngredientCommandBody body,
        CancellationToken ct)
    {
        var command = new UpdateRecipeIngredientCommand(recipeId, ingredientId, body);
        
        var result = await mediator.Send(command, ct);
        
        return result.ToActionResult();
    }
}