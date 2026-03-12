using BolluFlow.Application.Features.Dashboard.GetLowStockIngredients;
using BolluFlow.Application.Features.Dashboard.GetLowStockProducts;
using BolluFlow.Application.Features.Dashboard.GetLowStockSupplies;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class DashboardController(IMediator mediator) : ControllerBase
{
    [HttpGet("low-stock-ingredients")]
    public async Task<ActionResult<List<GetLowStockIngredientsResult>>> ListLowStockIngredients(CancellationToken ct)
    {
        var result = await mediator.Send(new GetLowStockIngredientsQuery(), ct);
        return result.ToActionResult();
    }

    [HttpGet("low-stock-supplies")]
    public async Task<ActionResult<List<GetLowStockSuppliesResult>>> ListLowStockSupplies(CancellationToken ct)
    {
        var result = await mediator.Send(new GetLowStockSuppliesQuery(), ct);
        return result.ToActionResult();
    }

    [HttpGet("low-stock-products")]
    public async Task<ActionResult<List<GetLowStockProductsResult>>> ListLowStockProducts(CancellationToken ct)
    {
        var result = await mediator.Send(new GetLowStockProductsQuery(), ct);
        return result.ToActionResult();
    }
}
