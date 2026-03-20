using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Financial;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Enums;
using BolluFlow.Domain.Errors;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Financial.ListFinancialEntries;

public class ListFinancialEntriesHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<ListFinancialEntriesQuery, ResultOf<PagedResult<FinancialEntryModel>>>
{
    public async Task<ResultOf<PagedResult<FinancialEntryModel>>> Handle(ListFinancialEntriesQuery request, CancellationToken cancellationToken)
    {
        if (request.Page <= 0 || request.PageSize <= 0)
            return ResultOf<PagedResult<FinancialEntryModel>>.BadRequest(FinancialErrors.InvalidPagination);

        if (!FinancialMonth.TryParse(request.Month, out var month))
            return ResultOf<PagedResult<FinancialEntryModel>>.BadRequest(FinancialErrors.InvalidMonthFormat);

        if (!FinancialFilterParser.TryParseType(request.Type, out var type)
            || !FinancialFilterParser.TryParseStatus(request.Status, out var status)
            || !FinancialFilterParser.TryParseSource(request.Source, out var source))
            return ResultOf<PagedResult<FinancialEntryModel>>.BadRequest(FinancialErrors.InvalidFilter);

        await FinancialSalesEntrySync.EnsureSalesEntries(db, user.Id, cancellationToken);
        await FinancialStockPurchaseSync.EnsureFinancialEntries(db, user.Id, cancellationToken);
        await FinancialFixedAccountSync.EnsureEntriesForMonth(db, user.Id, month, cancellationToken);

        var query = db.FinancialEntries
            .Where(x => x.UserId == user.Id && x.Date >= month.Start && x.Date <= month.End)
            .AsQueryable();

        query = status is null
            ? query.Where(x => x.Status != FinancialEntryStatus.Excluded)
            : query.Where(x => x.Status == status.Value);

        if (type is not null)
            query = query.Where(x => x.Type == type.Value);

        if (source is not null)
            query = query.Where(x => x.Source == source.Value);

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return ResultOf<PagedResult<FinancialEntryModel>>.Success(new PagedResult<FinancialEntryModel>
        {
            Items = items.Select(x => x.ToModel()).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize)
        });
    }
}
