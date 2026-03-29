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
        var query = db.FinancialEntries.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Type))
        {
            if (!Enum.TryParse<FinancialEntryType>(request.Type, true, out var type))
                return ResultOf<List<FinancialEntryModel>>.BadRequest(new("FINANCIAL_TYPE_INVALID", "type must be income or expense"));

            query = query.Where(x => x.Type == type);
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
