using BolluFlow.Application.Features.Dashboard.GetLowStockIngredients;
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
        var query = new GetLowStockIngredientsQuery();
        
        var result = await mediator.Send(query, ct);
        
        return result.ToActionResult();
    }
}