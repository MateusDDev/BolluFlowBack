using BolluFlow.Application.Features.Reports.OrderReport;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ReportsController(IMediator mediator) : ControllerBase
{
    [HttpGet("order/{id:long}")]
    public async Task<IActionResult> GetOrderReport([FromRoute] long id, CancellationToken ct)
    {
        var query = new OrderReportQuery(id);
        
        var result = await mediator.Send(query, ct);

        return result.ToPdfResult();
    }
}