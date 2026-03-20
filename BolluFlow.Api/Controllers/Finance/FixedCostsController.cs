using BolluFlow.Application.Features.Finance.CreateFixedCost;
using BolluFlow.Application.Features.Finance.DeleteFixedCost;
using BolluFlow.Application.Features.Finance.ListFixedCosts;
using BolluFlow.Application.Features.Finance.Shared;
using BolluFlow.Application.Features.Finance.UpdateFixedCost;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers.Finance;

[ApiController]
[Route("api/finance/fixed-costs")]
[Authorize]
public class FixedCostsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<FixedCostDto>>> List(CancellationToken ct)
        => (await mediator.Send(new ListFixedCostsQuery(), ct)).ToActionResult();

    [HttpPost]
    public async Task<ActionResult<FixedCostDto>> Create([FromBody] CreateFixedCostCommand command, CancellationToken ct)
        => (await mediator.Send(command, ct)).ToActionResult();

    public record UpdateFixedCostBody(string Name, decimal Amount, string StartMonth, string? EndMonth, string? Notes);

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<FixedCostDto>> Update([FromRoute] Guid id, [FromBody] UpdateFixedCostBody body, CancellationToken ct)
        => (await mediator.Send(new UpdateFixedCostCommand(id, body.Name, body.Amount, body.StartMonth, body.EndMonth, body.Notes), ct)).ToActionResult();

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<object>> Delete([FromRoute] Guid id, [FromQuery] string mode, [FromQuery] string month, CancellationToken ct)
        => (await mediator.Send(new DeleteFixedCostCommand(id, mode, month), ct)).ToActionResult();
}
