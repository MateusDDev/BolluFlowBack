using BolluFlow.Application.Features.Ingredients.CreateIngredient;
using BolluFlow.Application.Features.Ingredients.DeleteIngredient;
using BolluFlow.Application.Features.Ingredients.GetIngredientDetails;
using BolluFlow.Application.Features.Ingredients.ListIngredients;
using BolluFlow.Application.Features.Ingredients.UpdateIngredient;
using BolluFlow.Application.Models;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class IngredientsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CreateIngredientResult>> CreateIngredient([FromBody] CreateIngredientCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<ActionResult<List<IngredientModel>>> ListIngredients(CancellationToken ct)
    {
        var query = new ListIngredientsQuery();
        
        var result = await mediator.Send(query, ct);
        
        return result.ToActionResult();
    }

    [HttpGet("{ingredientId:long}")]
    public async Task<ActionResult<IngredientModel>> GetIngredientById([FromRoute] long ingredientId, CancellationToken ct)
    {
        var query = new GetIngredientDetailsQuery(ingredientId);
        
        var result = await mediator.Send(query, ct);
        
        return result.ToActionResult();
    }

    [HttpPut("{ingredientId:long}")]
    public async Task<ActionResult<IngredientModel>> UpdateIngredient(
        [FromRoute] long ingredientId,
        [FromBody] UpdateIngredientCommandBody body,
        CancellationToken ct)
    {
        var command = new UpdateIngredientCommand(ingredientId, body);
        
        var result = await mediator.Send(command, ct);
        
        return result.ToActionResult();
    }

    [HttpDelete("{ingredientId:long}")]
    public async Task<ActionResult> DeleteIngredient([FromRoute] long ingredientId, CancellationToken ct)
    {
        var command = new DeleteIngredientCommand(ingredientId);
        
        var result = await mediator.Send(command, ct);
        
        return result.ToActionResult();
    }
}