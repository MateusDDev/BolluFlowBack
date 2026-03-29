using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Finance.ListFinancialEntries;

public class ListFinancialEntriesHandler(ApplicationDbContext db)
    : IRequestHandler<ListFinancialEntriesQuery, ResultOf<List<FinancialEntryModel>>>
{
    public async Task<ResultOf<List<FinancialEntryModel>>> Handle(ListFinancialEntriesQuery request, CancellationToken cancellationToken)
    {
        if (request.Month.HasValue ^ request.Year.HasValue)
            return ResultOf<List<FinancialEntryModel>>.BadRequest(new("FINANCIAL_PERIOD_INVALID", "month and year must be informed together"));

        if (request.Month is < 1 or > 12)
            return ResultOf<List<FinancialEntryModel>>.BadRequest(new("FINANCIAL_MONTH_INVALID", "month must be between 1 and 12"));

        if (request.Year is < 2000 or > 2100)
            return ResultOf<List<FinancialEntryModel>>.BadRequest(new("FINANCIAL_YEAR_INVALID", "year must be between 2000 and 2100"));

        var query = db.FinancialEntries.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Type))
        {
            if (!Enum.TryParse<FinancialEntryType>(request.Type, true, out var type))
                return ResultOf<List<FinancialEntryModel>>.BadRequest(new("FINANCIAL_TYPE_INVALID", "type must be income or expense"));

            query = query.Where(x => x.Type == type);
        }

        if (request.Month.HasValue && request.Year.HasValue)
        {
            var monthStart = new DateTime(request.Year.Value, request.Month.Value, 1, 0, 0, 0, DateTimeKind.Utc);
            var nextMonth = monthStart.AddMonths(1);
            query = query.Where(x => x.CreatedAt >= monthStart && x.CreatedAt < nextMonth);
        }

        var entries = await query
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new FinancialEntryModel
            {
                Id = x.Id,
                Type = x.Type == FinancialEntryType.Income ? "income" : "expense",
                Amount = x.Amount,
                Description = x.Description,
                Origin = x.Origin == FinancialOriginType.IngredientPurchase
                    ? "ingredient_purchase"
                    : x.Origin == FinancialOriginType.SupplyPurchase
                        ? "supply_purchase"
                        : x.Origin == FinancialOriginType.StockAdjustment
                            ? "stock_adjustment"
                            : x.Origin == FinancialOriginType.Manual
                                ? "manual"
                                : "order",
                OriginId = x.OriginId,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return ResultOf<List<FinancialEntryModel>>.Success(entries);
    }
}
