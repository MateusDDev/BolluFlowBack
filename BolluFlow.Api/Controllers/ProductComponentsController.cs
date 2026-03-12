using BolluFlow.Application.Features.Products.Components.CreateProductComponent;
using BolluFlow.Application.Features.Products.Components.DeleteProductComponent;
using BolluFlow.Application.Features.Products.Components.GetProductComponents;
using BolluFlow.Application.Features.Products.Components.GetProductComponentsCatalog;
using BolluFlow.Application.Features.Products.Components.UpdateProductComponent;
using BolluFlow.Application.Models;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers.Products;

[ApiController]
[Route("products/{productId:long}/components")]
[Authorize]
public class ProductComponentsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ProductComponentModel>>> List(long productId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetProductComponentsQuery(productId), ct);
        return result.ToActionResult();
    }

    [HttpGet("catalog")]
    public async Task<ActionResult<List<ProductComponentCatalogItemModel>>> Catalog(long productId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetProductComponentsCatalogQuery(productId), ct);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<ActionResult> Create(long productId, [FromBody] CreateProductComponentCommandBody body, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateProductComponentCommand(productId, body), ct);
        return result.ToActionResult();
    }

    [HttpPut("{componentId:long}")]
    public async Task<ActionResult> Update(long productId, long componentId, [FromBody] UpdateProductComponentCommandBody body, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateProductComponentCommand(productId, componentId, body), ct);
        return result.ToActionResult();
    }

    [HttpDelete("{componentId:long}")]
    public async Task<ActionResult> Delete(long productId, long componentId, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteProductComponentCommand(productId, componentId), ct);
        return result.ToActionResult();
    }
}
