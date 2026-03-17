using BolluFlow.Application.Features.Financial.CreateFinancialEntry;
using BolluFlow.Application.Features.Financial.CreateFixedAccount;
using BolluFlow.Application.Features.Financial.DeleteFinancialEntry;
using BolluFlow.Application.Features.Financial.GenerateFixedAccountEntries;
using BolluFlow.Application.Features.Financial.GetFinancialOverview;
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
    [HttpPost("entries")]
    [ProducesResponseType(typeof(FinancialEntryModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<FinancialEntryModel>> CreateEntry([FromBody] CreateFinancialEntryCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpGet("entries")]
    [ProducesResponseType(typeof(PagedResult<FinancialEntryModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<FinancialEntryModel>>> ListEntries(
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        [FromQuery] FinancialEntryType? type,
        [FromQuery] FinancialEntryStatus? status,
        [FromQuery] bool? isFixed,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new ListFinancialEntriesQuery(startDate, endDate, type, status, isFixed, page, pageSize), ct);
        return result.ToActionResult();
    }

    [HttpPatch("entries/{entryId:long}/status")]
    [ProducesResponseType(typeof(FinancialEntryModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<FinancialEntryModel>> UpdateStatus(long entryId, [FromBody] UpdateFinancialEntryStatusBody body, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateFinancialEntryStatusCommand(entryId, body.Status), ct);
        return result.ToActionResult();
    }

    [HttpDelete("entries/{entryId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteEntry(long entryId, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteFinancialEntryCommand(entryId), ct);
        return result.ToActionResult();
    }

    [HttpGet("sales-entries")]
    [ProducesResponseType(typeof(PagedResult<SaleFinancialEntryModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<SaleFinancialEntryModel>>> ListSalesEntries(
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new ListSalesEntriesQuery(startDate, endDate, page, pageSize), ct);
        return result.ToActionResult();
    }

    [HttpGet("overview")]
    [ProducesResponseType(typeof(FinancialOverviewModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<FinancialOverviewModel>> GetOverview([FromQuery] string month, CancellationToken ct)
    {
        var result = await mediator.Send(new GetFinancialOverviewQuery(month), ct);
        return result.ToActionResult();
    }

    [HttpPost("fixed-accounts")]
    [ProducesResponseType(typeof(FixedAccountModel), StatusCodes.Status200OK)]
    public async Task<ActionResult<FixedAccountModel>> CreateFixedAccount([FromBody] CreateFixedAccountCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.ToActionResult();
    }

    [HttpGet("fixed-accounts")]
    [ProducesResponseType(typeof(PagedResult<FixedAccountModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<FixedAccountModel>>> ListFixedAccounts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new ListFixedAccountsQuery(page, pageSize), ct);
        return result.ToActionResult();
    }

    [HttpPost("fixed-accounts/generate-month")]
    [ProducesResponseType(typeof(List<FinancialEntryModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<FinancialEntryModel>>> GenerateMonth([FromQuery] string month, CancellationToken ct)
    {
        var result = await mediator.Send(new GenerateFixedAccountEntriesCommand(month), ct);
        return result.ToActionResult();
    }
}

public record UpdateFinancialEntryStatusBody(FinancialEntryStatus Status);
