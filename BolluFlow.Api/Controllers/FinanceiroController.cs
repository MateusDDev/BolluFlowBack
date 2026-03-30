using BolluFlow.Application.Features.Finance.CreateFinancialEntry;
using BolluFlow.Application.Features.Finance.DeleteFinancialEntry;
using BolluFlow.Application.Features.Finance.GetFinancialBalance;
using BolluFlow.Application.Features.Finance.ListFinancialEntries;
using BolluFlow.Application.Models;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Api.Controllers;

[ApiController]
[Route("financial")]
[Authorize]
public class FinanceiroController(IMediator mediator) : ControllerBase
{
    [HttpPost("entries")]
    public async Task<ActionResult<FinancialEntryModel>> Create(
        [FromBody] CreateFinancialEntryCommand command,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        if (result.IsSuccessful)
            return StatusCode(201, result.Value);

        return result.ToActionResult();
    }

    [HttpGet("entries")]
    public async Task<ActionResult<List<FinancialEntryModel>>> List([FromQuery] string? type, [FromQuery] int? month, [FromQuery] int? year, CancellationToken ct)
    {
        var result = await mediator.Send(new ListFinancialEntriesQuery(type, month, year), ct);
        return result.ToActionResult();
    }


    [HttpDelete("entries/{id:long}")]
    public async Task<ActionResult> Delete(long id, [FromQuery] bool revertStock = false, CancellationToken ct = default)
    {
        var result = await mediator.Send(new DeleteFinancialEntryCommand(id, revertStock), ct);
        return result.ToActionResult();
    }

    [HttpGet("balance")]
    public async Task<ActionResult<FinancialBalanceModel>> Balance([FromQuery] int? month, [FromQuery] int? year, CancellationToken ct)
    {
        var result = await mediator.Send(new GetFinancialBalanceQuery(month, year), ct);
        return result.ToActionResult();
    }
}
