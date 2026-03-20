using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Recipes.Conversions.CreateRecipeConversion;
using BolluFlow.Application.Features.Recipes.Conversions.DeleteRecipeConversion;
using BolluFlow.Application.Features.Recipes.Conversions.ListRecipeConversions;
using BolluFlow.Application.Features.Recipes.Conversions.UpdateRecipeConversion;
using BolluFlow.Application.Models;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers.Recipes;

[ApiController]
[Route("api/receitas/{recipeId:long}/conversoes")]
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
    public async Task<ActionResult<Result>> Create([FromRoute] long recipeId, [FromBody] RecipeConversionBody body, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateRecipeConversionCommand(recipeId, body), ct);
        return result.ToActionResult();
    }

    [HttpPut("{conversionId:long}")]
    public async Task<ActionResult<Result>> Update([FromRoute] long recipeId, [FromRoute] long conversionId, [FromBody] RecipeConversionBody body, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateRecipeConversionCommand(recipeId, conversionId, body), ct);
        return result.ToActionResult();
    }

    [HttpDelete("{conversionId:long}")]
    public async Task<ActionResult<Result>> Delete([FromRoute] long recipeId, [FromRoute] long conversionId, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteRecipeConversionCommand(recipeId, conversionId), ct);
        return result.ToActionResult();
    }
}
