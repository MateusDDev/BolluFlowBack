using BolluFlow.Application.Features.Recipes.Create;
using BolluFlow.Application.Features.Recipes.GetRecipeDetails;
using BolluFlow.Application.Features.Recipes.ListRecipes;
using BolluFlow.Application.Features.Recipes.SimulatePricing;
using BolluFlow.Application.Features.Recipes.UpdateRecipe;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class RecipesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateRecipe([FromBody] CreateRecipeCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpPost("simulate-pricing")]
    public async Task<IActionResult> SimulatePricing([FromBody] SimulatePricingCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpPut("{recipeId:long}")]
    public async Task<IActionResult> UpdateRecipe(
        [FromRoute] long recipeId,
        [FromBody] UpdateRecipeCommandBody body,
        CancellationToken ct)
    {
        var command = new UpdateRecipeCommand(recipeId, body);
        
        var result = await mediator.Send(command, ct);
        
        return result.ToActionResult();
    }

    [HttpGet("{recipeId:long}")]
    public async Task<IActionResult> GetRecipeById([FromRoute] long recipeId, CancellationToken ct)
    {
        var query = new GetRecipeDetailsQuery(recipeId);
        
        var result = await mediator.Send(query, ct);
        
        return result.ToActionResult();
    }
    
    [HttpGet]
    public async Task<IActionResult> ListRecipes(CancellationToken ct)
    {
        var query = new ListRecipesQuery();
        
        var result = await mediator.Send(query, ct);
        
        return result.ToActionResult();
    }
}
    