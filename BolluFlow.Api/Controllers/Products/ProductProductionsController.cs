using BolluFlow.Application.Features.Products.Productions.RecordProduction;
using BolluFlow.Application.Models;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers.Products;

[ApiController]
[Route("products/{productId:long}/productions")]
[Authorize]
public class ProductProductionsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ProductProductionModel>> Create(
        [FromRoute] long productId,
        [FromBody] RecordProductProductionCommandBody body,
        CancellationToken ct)
    {
        var result = await mediator.Send(new RecordProductProductionCommand(productId, body), ct);

        if (result.IsSuccessful)
            return StatusCode(201, result.Value);

        return result.ToActionResult();
    }
}
