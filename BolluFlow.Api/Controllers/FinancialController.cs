using BolluFlow.Application.Features.Financial.CreateFinancialEntry;
using BolluFlow.Application.Features.Financial.CreateFixedAccount;
using BolluFlow.Application.Features.Financial.DeleteFinancialEntry;
using BolluFlow.Application.Features.Financial.DeleteFixedAccountRange;
using BolluFlow.Application.Features.Financial.ExcludeFinancialEntry;
using BolluFlow.Application.Features.Financial.GetFinancialOverview;
using BolluFlow.Application.Features.Financial.ListAutoStockPurchases;
using BolluFlow.Application.Features.Financial.ListFinancialEntries;
using BolluFlow.Application.Features.Financial.ListFixedAccounts;
using BolluFlow.Application.Features.Financial.ListSalesEntries;
using BolluFlow.Application.Features.Financial.UpdateFinancialEntry;
using BolluFlow.Application.Features.Financial.UpdateFinancialEntryStatus;
using BolluFlow.Application.Features.Financial.UpdateFixedAccount;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Enums;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers;

[ApiController]
[Route("api/financeiro")]
[Authorize]
public class FinancialController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Lista os lançamentos financeiros do mês selecionado.
    /// </summary>
    [HttpGet("lancamentos")]
    [ProducesResponseType(typeof(PagedResult<FinancialEntryModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<FinancialEntryModel>>> ListLancamentos(
        [FromQuery] string month,
        [FromQuery] string? type,
        [FromQuery] string? source,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new ListFinancialEntriesQuery(month, type, source, status, page, pageSize), ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Cria um lançamento manual.
    /// </summary>
    /// <remarks>
    /// Exemplo:
    ///
    ///     POST /api/financeiro/lancamentos
    ///     {
    ///       "description": "Compra emergencial de embalagem",
    ///       "type": "expense",
    ///       "category": "Insumos",
    ///       "amount": 120.5,
    ///       "date": "2026-03-19",
    ///       "status": "paid",
    ///       "isFixed": false
    ///     }
    /// </remarks>
    [HttpPost("lancamentos")]
    [ProducesResponseType(typeof(FinancialEntryModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<FinancialEntryModel>> CreateLancamento([FromBody] CreateFinancialEntryCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpPut("lancamentos/{id:long}")]
    [ProducesResponseType(typeof(FinancialEntryModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<FinancialEntryModel>> UpdateLancamento(long id, [FromBody] UpdateFinancialEntryBody body, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateFinancialEntryCommand(id, body.Description, body.Type, body.Category, body.Amount, body.Date, body.Status, body.IsFixed, body.Notes), ct);
        return result.ToActionResult();
    }

    [HttpPatch("lancamentos/{id:long}/status")]
    [ProducesResponseType(typeof(FinancialEntryModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<FinancialEntryModel>> UpdateStatus(long id, [FromBody] UpdateFinancialEntryStatusBody body, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateFinancialEntryStatusCommand(id, body.Status), ct);
        return result.ToActionResult();
    }

    [HttpDelete("lancamentos/{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteLancamento(long id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteFinancialEntryCommand(id), ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Exclui logicamente um lançamento do extrato preservando a auditoria.
    /// </summary>
    [HttpPost("lancamentos/{id:long}/exclude")]
    [ProducesResponseType(typeof(FinancialEntryModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<FinancialEntryModel>> ExcludeLancamento(long id, [FromBody] ExcludeFinancialEntryBody body, CancellationToken ct)
    {
        var result = await mediator.Send(new ExcludeFinancialEntryCommand(id, body.Reason, body.PreserveSource), ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Lista as entradas automáticas originadas por pedidos entregues.
    /// </summary>
    [HttpGet("lancamentos/auto-sales")]
    [ProducesResponseType(typeof(PagedResult<AutoFinancialEntryModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AutoFinancialEntryModel>>> ListAutoSales([FromQuery] string month, [FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
    {
        var result = await mediator.Send(new ListSalesEntriesQuery(month, page, pageSize), ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Lista as despesas automáticas originadas por compras/reposições de estoque.
    /// </summary>
    [HttpGet("lancamentos/auto-stock")]
    [ProducesResponseType(typeof(PagedResult<AutoFinancialEntryModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AutoFinancialEntryModel>>> ListAutoStock([FromQuery] string month, [FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
    {
        var result = await mediator.Send(new ListAutoStockPurchasesQuery(month, page, pageSize), ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Retorna o resumo mensal com saldos acumulados até o fim do mês selecionado.
    /// </summary>
    [HttpGet("resumo")]
    [ProducesResponseType(typeof(FinancialOverviewModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<FinancialOverviewModel>> GetResumo([FromQuery] string month, CancellationToken ct)
    {
        var result = await mediator.Send(new GetFinancialOverviewQuery(month), ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Cria uma conta fixa recorrente a partir do mês inicial informado.
    /// </summary>
    [HttpPost("contas-fixas")]
    [ProducesResponseType(typeof(FixedAccountModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<FixedAccountModel>> CreateContaFixa([FromBody] CreateFixedAccountCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpGet("contas-fixas")]
    [ProducesResponseType(typeof(PagedResult<FixedAccountModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<FixedAccountModel>>> ListContasFixas([FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
    {
        var result = await mediator.Send(new ListFixedAccountsQuery(page, pageSize), ct);
        return result.ToActionResult();
    }

    [HttpPut("contas-fixas/{id:long}")]
    [ProducesResponseType(typeof(FixedAccountModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<FixedAccountModel>> UpdateContaFixa(long id, [FromBody] UpdateFixedAccountBody body, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateFixedAccountCommand(id, body.Description, body.Category, body.Amount, body.StartMonth, body.DefaultStatus), ct);
        return result.ToActionResult();
    }

    [HttpPost("contas-fixas/{id:long}/delete-range")]
    [ProducesResponseType(typeof(FixedAccountModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<FixedAccountModel>> DeleteContaFixaRange(long id, [FromBody] DeleteFixedAccountRangeBody body, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteFixedAccountRangeCommand(id, body.Mode, body.Month), ct);
        return result.ToActionResult();
    }
}

public record UpdateFinancialEntryBody(
    string Description,
    FinancialEntryType Type,
    string Category,
    decimal Amount,
    DateOnly Date,
    FinancialEntryStatus Status,
    bool IsFixed,
    string? Notes);

public record UpdateFinancialEntryStatusBody(FinancialEntryStatus Status);
public record ExcludeFinancialEntryBody(string Reason, bool PreserveSource);
public record UpdateFixedAccountBody(string Description, string Category, decimal Amount, string StartMonth, FinancialEntryStatus DefaultStatus);
public record DeleteFixedAccountRangeBody(DeletionMode Mode, string? Month);
