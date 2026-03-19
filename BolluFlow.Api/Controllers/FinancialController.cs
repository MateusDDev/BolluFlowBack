using BolluFlow.Application.Features.Financial.CreateFinancialEntry;
using BolluFlow.Application.Features.Financial.CreateFixedAccount;
using BolluFlow.Application.Features.Financial.DeleteFinancialEntry;
using BolluFlow.Application.Features.Financial.GenerateFixedAccountEntries;
using BolluFlow.Application.Features.Financial.GetFinancialOverview;
using BolluFlow.Application.Features.Financial.ListAutoStockPurchases;
using BolluFlow.Application.Features.Financial.ListFinancialEntries;
using BolluFlow.Application.Features.Financial.ListFixedAccounts;
using BolluFlow.Application.Features.Financial.ListSalesEntries;
using BolluFlow.Application.Features.Financial.UpdateFinancialEntryStatus;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Enums;
using BolluFlow.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolluFlow.Controllers;

[ApiController]
[Route("financial")]
[Authorize]
public class FinancialController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Cria um lançamento financeiro manual.
    /// </summary>
    /// <remarks>
    /// Exemplo de payload:
    ///
    ///     POST /financial/entries
    ///     {
    ///       "description": "Compra de embalagem",
    ///       "type": "Expense",
    ///       "category": "Insumos",
    ///       "amount": 189.5,
    ///       "date": "2026-03-12",
    ///       "status": "Paid",
    ///       "isFixed": false,
    ///       "notes": "Fornecedor XP"
    ///     }
    /// </remarks>
    [HttpPost("entries")]
    [ProducesResponseType(typeof(FinancialEntryModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<FinancialEntryModel>> CreateEntry([FromBody] CreateFinancialEntryCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Lista lançamentos financeiros do mês informado.
    /// </summary>
    [HttpGet("entries")]
    [ProducesResponseType(typeof(PagedResult<FinancialEntryModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<FinancialEntryModel>>> ListEntries(
        [FromQuery] string month,
        [FromQuery] FinancialEntryType? type,
        [FromQuery] FinancialEntryStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new ListFinancialEntriesQuery(month, type, status, page, pageSize), ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Atualiza o status de um lançamento financeiro.
    /// </summary>
    /// <remarks>
    /// Exemplo de payload:
    ///
    ///     PATCH /financial/entries/123/status
    ///     {
    ///       "status": "Paid"
    ///     }
    /// </remarks>
    [HttpPatch("entries/{entryId:long}/status")]
    [ProducesResponseType(typeof(FinancialEntryModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<FinancialEntryModel>> UpdateStatus(long entryId, [FromBody] UpdateFinancialEntryStatusBody body, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateFinancialEntryStatusCommand(entryId, body.Status), ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Exclui um lançamento manual.
    /// </summary>
    [HttpDelete("entries/{entryId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteEntry(long entryId, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteFinancialEntryCommand(entryId), ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Lista entradas automáticas de vendas do mês.
    /// </summary>
    [HttpGet("entries/auto-sales")]
    [ProducesResponseType(typeof(PagedResult<AutoFinancialEntryModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AutoFinancialEntryModel>>> ListAutoSales(
        [FromQuery] string month,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new ListSalesEntriesQuery(month, page, pageSize), ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Lista saídas automáticas de compras de estoque do mês.
    /// </summary>
    [HttpGet("entries/auto-stock-purchases")]
    [ProducesResponseType(typeof(PagedResult<AutoFinancialEntryModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AutoFinancialEntryModel>>> ListAutoStockPurchases(
        [FromQuery] string month,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new ListAutoStockPurchasesQuery(month, page, pageSize), ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Retorna o resumo financeiro mensal.
    /// </summary>
    /// <remarks>
    /// Exemplo de resposta:
    ///
    ///     {
    ///       "month": "2026-03",
    ///       "currentBalance": 4200.0,
    ///       "forecastBalance": 3950.0,
    ///       "totalIncomesPaid": 9000.0,
    ///       "totalExpensesPaid": 4800.0,
    ///       "pendingToReceive": 700.0,
    ///       "pendingToPay": 950.0,
    ///       "fixedCostsInMonth": 2100.0
    ///     }
    /// </remarks>
    [HttpGet("overview")]
    [ProducesResponseType(typeof(FinancialOverviewModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<FinancialOverviewModel>> GetOverview([FromQuery] string month, CancellationToken ct)
    {
        var result = await mediator.Send(new GetFinancialOverviewQuery(month), ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Cria uma conta fixa.
    /// </summary>
    /// <remarks>
    /// Exemplo de payload:
    ///
    ///     POST /financial/fixed-accounts
    ///     {
    ///       "description": "Conta de luz",
    ///       "category": "Utilidades",
    ///       "defaultAmount": 320.0,
    ///       "dueDay": 10,
    ///       "isActive": true
    ///     }
    /// </remarks>
    [HttpPost("fixed-accounts")]
    [ProducesResponseType(typeof(FixedAccountModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<FixedAccountModel>> CreateFixedAccount([FromBody] CreateFixedAccountCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Lista contas fixas cadastradas.
    /// </summary>
    [HttpGet("fixed-accounts")]
    [ProducesResponseType(typeof(PagedResult<FixedAccountModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<FixedAccountModel>>> ListFixedAccounts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new ListFixedAccountsQuery(page, pageSize), ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Gera os lançamentos mensais pendentes das contas fixas.
    /// </summary>
    [HttpPost("fixed-accounts/generate-month")]
    [ProducesResponseType(typeof(List<FinancialEntryModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<FinancialEntryModel>>> GenerateMonth([FromQuery] string month, CancellationToken ct)
    {
        var result = await mediator.Send(new GenerateFixedAccountEntriesCommand(month), ct);
        return result.ToActionResult();
    }
}

public record UpdateFinancialEntryStatusBody(FinancialEntryStatus Status);
