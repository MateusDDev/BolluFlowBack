using BolluFlow.Application.Features.Products.AdjustStock;
using BolluFlow.Application.Models;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers.Products;

[ApiController]
[Route("products/{productId:long}/stock")]
[Authorize]
public class ProductStockController(IMediator mediator) : ControllerBase
{
    [HttpPost("adjustment")]
    public async Task<ActionResult<ProductStockAdjustmentModel>> Adjust(
        [FromRoute] long productId,
        [FromBody] AdjustProductStockCommandBody body,
        CancellationToken ct)
    {
        var result = await mediator.Send(new AdjustProductStockCommand(productId, body), ct);

        if (result.IsSuccessful)
            return StatusCode(201, result.Value);

        return result.ToActionResult();
    }
}
