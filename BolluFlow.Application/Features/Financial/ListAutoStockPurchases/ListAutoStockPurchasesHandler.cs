using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Financial;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Enums;
using BolluFlow.Domain.Errors;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Financial.ListAutoStockPurchases;

public class ListAutoStockPurchasesHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<ListAutoStockPurchasesQuery, ResultOf<PagedResult<AutoFinancialEntryModel>>>
{
    public async Task<ResultOf<PagedResult<AutoFinancialEntryModel>>> Handle(ListAutoStockPurchasesQuery request, CancellationToken cancellationToken)
    {
        if (request.Page <= 0 || request.PageSize <= 0)
            return ResultOf<PagedResult<AutoFinancialEntryModel>>.BadRequest(FinancialErrors.InvalidPagination);

        if (!FinancialMonth.TryParse(request.Month, out var month))
            return ResultOf<PagedResult<AutoFinancialEntryModel>>.BadRequest(FinancialErrors.InvalidMonthFormat);

        await FinancialStockPurchaseSync.EnsureFinancialEntries(db, user.Id, cancellationToken);

        var query = db.FinancialEntries
            .Where(x => x.UserId == user.Id && x.Source == FinancialEntrySource.AutoStockPurchase && x.Date >= month.Start && x.Date <= month.End);

        var totalItems = await query.CountAsync(cancellationToken);
        var entries = await query
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return ResultOf<PagedResult<AutoFinancialEntryModel>>.Success(new PagedResult<AutoFinancialEntryModel>
        {
            Items = entries.Select(x => new AutoFinancialEntryModel
            {
                Id = $"stock-purchase-{x.ReferenceId}",
                Description = x.Description,
                Type = x.Type,
                Category = x.Category,
                Amount = x.Amount,
                Date = x.Date,
                Status = x.Status,
                Source = x.Source,
                ReferenceId = x.ReferenceId ?? 0
            }).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize)
        });
    }
}
