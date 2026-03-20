using BolluFlow.Application.Features.Finance.GetOverview;
using BolluFlow.Application.Features.Finance.Shared;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers.Finance;

[ApiController]
[Route("api/finance/overview")]
[Authorize]
public class FinanceOverviewController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<FinanceOverviewDto>> GetOverview([FromQuery] string month, CancellationToken ct)
        => (await mediator.Send(new GetFinanceOverviewQuery(month), ct)).ToActionResult();
}
