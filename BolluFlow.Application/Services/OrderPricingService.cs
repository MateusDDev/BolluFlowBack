using BolluFlow.Domain.Entities;
using BolluFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Services;

public record ProductPricing(Product Product, decimal UnitCost);

public static class OrderPricingService
{
    public static async Task<Dictionary<long, ProductPricing>> LoadAsync(
        ApplicationDbContext db,
        long userId,
        IEnumerable<long> productIds,
        CancellationToken cancellationToken)
    {
        var ids = productIds.Distinct().ToList();

        var products = await db.Products
            .AsNoTracking()
            .Where(x => x.UserId == userId && ids.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var components = await db.ProductComponents
            .AsNoTracking()
            .Include(x => x.Ingredient)
            .Include(x => x.Supply)
            .Include(x => x.Recipe)
            .ThenInclude(x => x!.RecipeIngredients)
            .ThenInclude(x => x.Ingredient)
            .Where(x => ids.Contains(x.ProductId))
            .ToListAsync(cancellationToken);

        var costsByProduct = components
            .GroupBy(x => x.ProductId)
            .ToDictionary(x => x.Key, x => x.Sum(ProductComponentCostService.CalculateComponentCost));

        return products.ToDictionary(
            x => x.Id,
            x => new ProductPricing(x, costsByProduct.GetValueOrDefault(x.Id, 0m)));
    }
}
