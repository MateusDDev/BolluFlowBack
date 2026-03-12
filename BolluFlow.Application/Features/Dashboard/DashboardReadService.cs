using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Dashboard;

public sealed class DashboardReadService(ApplicationDbContext db, CurrentUser currentUser) : IDashboardReadService
{
    public async Task<IReadOnlyList<StockAlertResponse>> GetStockAlertsAsync(int limit, CancellationToken ct)
    {
        var effectiveLimit = Math.Clamp(limit, 1, 50);
        const decimal warningFactor = 1.2m;

        var productAlerts = db.Products
            .Where(x => x.UserId == currentUser.Id
                        && x.MinimumStockQuantity.HasValue
                        && x.MinimumStockQuantity.Value > 0
                        && x.StockQuantity <= (x.MinimumStockQuantity.Value * warningFactor))
            .Select(x => new
            {
                x.Id,
                x.Name,
                Category = "Product",
                x.StockQuantity,
                MinimumStockQuantity = x.MinimumStockQuantity!.Value
            });

        var ingredientAlerts = db.Ingredients
            .Where(x => x.UserId == currentUser.Id
                        && x.MinimumStockQuantity.HasValue
                        && x.MinimumStockQuantity.Value > 0
                        && x.StockQuantity <= (x.MinimumStockQuantity.Value * warningFactor))
            .Select(x => new
            {
                x.Id,
                x.Name,
                Category = "Ingredient",
                x.StockQuantity,
                MinimumStockQuantity = x.MinimumStockQuantity!.Value
            });

        var supplyAlerts = db.Supplies
            .Where(x => x.UserId == currentUser.Id
                        && x.MinimumStockQuantity.HasValue
                        && x.MinimumStockQuantity.Value > 0
                        && x.StockQuantity <= (x.MinimumStockQuantity.Value * warningFactor))
            .Select(x => new
            {
                x.Id,
                x.Name,
                Category = "Supply",
                x.StockQuantity,
                MinimumStockQuantity = x.MinimumStockQuantity!.Value
            });

        var rawAlerts = await productAlerts
            .Concat(ingredientAlerts)
            .Concat(supplyAlerts)
            .AsNoTracking()
            .ToListAsync(ct);

        return rawAlerts
            .Select(x => new StockAlertResponse(
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
    }

    public async Task<IReadOnlyList<OrdersByStatusResponse>> GetOrdersByStatusAsync(CancellationToken ct)
    {
        var grouped = await db.Orders
            .AsNoTracking()
            .Where(x => x.Customer.UserId == currentUser.Id)
            .GroupBy(x => x.Status)
            .Select(x => new { Status = x.Key, Total = x.Count() })
            .ToListAsync(ct);

        var groupedDictionary = grouped.ToDictionary(x => x.Status, x => x.Total);

        return Enum.GetValues<OrderStatus>()
            .Select(status => new OrdersByStatusResponse(
                status.ToString(),
                groupedDictionary.GetValueOrDefault(status, 0)))
            .ToList();
    }

    public async Task<IReadOnlyList<InventoryValueByTypeResponse>> GetInventoryValueByTypeAsync(CancellationToken ct)
    {
        // Official dashboard contract uses Product.FinalPrice as source of truth for product stock valuation.
        var productsTotal = await db.Products
            .AsNoTracking()
            .Where(x => x.UserId == currentUser.Id)
            .Select(x => x.StockQuantity * x.FinalPrice)
            .DefaultIfEmpty(0m)
            .SumAsync(ct);

        var ingredientsTotal = await db.Ingredients
            .AsNoTracking()
            .Where(x => x.UserId == currentUser.Id)
            .Select(x => x.StockQuantity * x.UnitCost)
            .DefaultIfEmpty(0m)
            .SumAsync(ct);

        var suppliesTotal = await db.Supplies
            .AsNoTracking()
            .Where(x => x.UserId == currentUser.Id)
            .Select(x => x.StockQuantity * x.UnitCost)
            .DefaultIfEmpty(0m)
            .SumAsync(ct);

        return
        [
            new InventoryValueByTypeResponse("Products", decimal.Round(productsTotal, 2)),
            new InventoryValueByTypeResponse("Ingredients", decimal.Round(ingredientsTotal, 2)),
            new InventoryValueByTypeResponse("Supplies", decimal.Round(suppliesTotal, 2))
        ];
    }

    public async Task<IReadOnlyList<RecentOrderResponse>> GetRecentOrdersAsync(int limit, CancellationToken ct)
    {
        var effectiveLimit = Math.Clamp(limit, 1, 20);

        return await db.Orders
            .AsNoTracking()
            .Where(x => x.Customer.UserId == currentUser.Id)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new RecentOrderResponse(
                x.Id,
                x.Number,
                x.Customer.Name,
                x.Status.ToString(),
                x.CreatedAt,
                x.Items.Sum(i => i.Quantity),
                decimal.Round(x.Items.Sum(i => i.UnitPrice * i.Quantity), 2)))
            .Take(effectiveLimit)
            .ToListAsync(ct);
    }
}
