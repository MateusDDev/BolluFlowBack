using BolluFlow.Application.Features.Products.CreateProduct;
using BolluFlow.Application.Features.Products.GetProductDetails;
using BolluFlow.Application.Features.Products.ListProducts;
using BolluFlow.Application.Features.Products.UpdateProduct;
using BolluFlow.Application.Features.Products.UpdateProductPricing;
using BolluFlow.Application.Models;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers.Products;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ProductModel>>> List(CancellationToken ct)
    {
        var result = await mediator.Send(new ListProductsQuery(), ct);
        return result.ToActionResult();
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ProductModel>> GetById(long id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetProductDetailsQuery(id), ct);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<ActionResult<CreateProductResult>> Create([FromBody] CreateProductCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpPatch("{id:long}")]
    public async Task<ActionResult<ProductModel>> Patch(long id, [FromBody] UpdateProductCommandBody body, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateProductCommand(id, body), ct);
        return result.ToActionResult();
    }

    [HttpPatch("{id:long}/pricing")]
    public async Task<ActionResult> PatchPricing(long id, [FromBody] UpdateProductPricingCommandBody body, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateProductPricingCommand(id, body), ct);
        return result.ToActionResult();
    }
}
