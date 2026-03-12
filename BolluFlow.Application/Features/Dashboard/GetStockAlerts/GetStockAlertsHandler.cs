using BolluFlow.Application.Abstractions;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Dashboard.GetStockAlerts;

public sealed class GetStockAlertsHandler(ApplicationDbContext db, CurrentUser currentUser)
    : IRequestHandler<GetStockAlertsQuery, ResultOf<List<GetStockAlertsResult>>>
{
    public async Task<ResultOf<List<GetStockAlertsResult>>> Handle(GetStockAlertsQuery request, CancellationToken cancellationToken)
    {
        var effectiveLimit = Math.Clamp(request.Limit ?? 10, 1, 50);
        const decimal warningFactor = 1.2m;

        var productAlerts = await db.Products
            .Where(x => x.UserId == currentUser.Id
                        && x.MinimumStockQuantity.HasValue
                        && x.MinimumStockQuantity > 0
                        && x.StockQuantity <= x.MinimumStockQuantity.Value * warningFactor)
            .Select(x => new
            {
                x.Id,
                x.Name,
                Category = "Product",
                x.StockQuantity,
                MinimumStockQuantity = x.MinimumStockQuantity!.Value
            }).ToListAsync(cancellationToken);

        var ingredientAlerts = await db.Ingredients
            .Where(x => x.UserId == currentUser.Id
                        && x.MinimumStockQuantity.HasValue
                        && x.MinimumStockQuantity > 0
                        && x.StockQuantity <= x.MinimumStockQuantity.Value * warningFactor)
            .Select(x => new
            {
                x.Id,
                x.Name,
                Category = "Ingredient",
                x.StockQuantity,
                MinimumStockQuantity = x.MinimumStockQuantity!.Value
            }).ToListAsync(cancellationToken);

        var supplyAlerts = await db.Supplies
            .Where(x => x.UserId == currentUser.Id
                        && x.MinimumStockQuantity.HasValue
                        && x.MinimumStockQuantity > 0
                        && x.StockQuantity <= x.MinimumStockQuantity.Value * warningFactor)
            .Select(x => new
            {
                x.Id,
                x.Name,
                Category = "Supply",
                x.StockQuantity,
                MinimumStockQuantity = x.MinimumStockQuantity!.Value
            }).ToListAsync(cancellationToken);

        var data = productAlerts
            .Concat(ingredientAlerts)
            .Concat(supplyAlerts)
            .ToList();

        var response = data
            .Select(x => new GetStockAlertsResult(
                x.Id,
                x.Name,
                x.Category,
                x.StockQuantity,
                x.MinimumStockQuantity,
                DashboardCalculations.GetCoveragePercent(x.StockQuantity, x.MinimumStockQuantity),
                DashboardCalculations.GetSeverity(x.StockQuantity, x.MinimumStockQuantity)))
            .OrderBy(x => x.Severity == "Critical" ? 0 : 1)
            .ThenBy(x => x.StockCoveragePercent)
            .ThenBy(x => x.Name)
            .Take(effectiveLimit)
            .ToList();

        return ResultOf<List<GetStockAlertsResult>>.Success(response);
    }
}
