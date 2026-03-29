using BolluFlow.Application.Features.Finance.CreateFinancialEntry;
using BolluFlow.Application.Features.Finance.GetFinancialBalance;
using BolluFlow.Application.Features.Finance.ListFinancialEntries;
using BolluFlow.Application.Models;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Api.Controllers;

[ApiController]
[Route("financeiro")]
[Authorize]
public class FinanceiroController(IMediator mediator) : ControllerBase
{
    [HttpPost("lancamentos")]
    public async Task<ActionResult<FinancialEntryModel>> Create(
        [FromBody] CreateFinancialEntryCommand command,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        if (result.IsSuccessful)
            return StatusCode(201, result.Value);

        return result.ToActionResult();
    }

    [HttpGet("lancamentos")]
    public async Task<ActionResult<List<FinancialEntryModel>>> List([FromQuery] string? tipo, CancellationToken ct)
    {
        var result = await mediator.Send(new ListFinancialEntriesQuery(tipo), ct);
        return result.ToActionResult();
    }

    [HttpGet("saldo")]
    public async Task<ActionResult<FinancialBalanceModel>> Balance(CancellationToken ct)
    {
        var result = await mediator.Send(new GetFinancialBalanceQuery(), ct);
        return result.ToActionResult();
    }
}
