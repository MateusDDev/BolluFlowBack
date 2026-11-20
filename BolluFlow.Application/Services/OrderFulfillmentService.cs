using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Services;

public record InsufficientProductStock(long ProductId, string ProductName, decimal Required, decimal Available);

public static class OrderFulfillmentService
{
    public static readonly DomainError InsufficientStockError =
        new("INSUFFICIENT_STOCK", "Estoque insuficiente para os produtos do pedido");

    public static Task<bool> AllowNegativeStockAsync(ApplicationDbContext db, long userId, CancellationToken cancellationToken)
    {
        return db.Users
            .Where(x => x.Id == userId)
            .Select(x => x.AllowNegativeStock || (x.EnabledModules & AppModule.Stock) == AppModule.None)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public static Task<Dictionary<long, Product>> LoadProductsAsync(ApplicationDbContext db, Order order, CancellationToken cancellationToken)
    {
        var productIds = order.Items.Select(x => x.ProductId).Distinct().ToList();

        return db.Products
            .Where(x => x.UserId == order.UserId && productIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, cancellationToken);
    }

    public static List<InsufficientProductStock> FindInsufficientStock(
        Order order,
        Dictionary<long, Product> products,
        bool allowNegativeStock)
    {
        return order.Items
            .Select(item => new { Item = item, Product = products.GetValueOrDefault(item.ProductId) })
            .Where(x => x.Product is null || !x.Product.HasEnoughStock(x.Item.Quantity, allowNegativeStock))
            .Select(x => new InsufficientProductStock(
                x.Item.ProductId,
                x.Product?.Name ?? "Produto não encontrado",
                x.Item.Quantity,
                x.Product?.StockQuantity ?? 0))
            .ToList();
    }

    public static void ConsumeStock(Order order, Dictionary<long, Product> products, bool allowNegativeStock)
    {
        foreach (var item in order.Items)
            products[item.ProductId].ConsumeStock(item.Quantity, allowNegativeStock);
    }

    public static async Task RegisterIncomeAsync(ApplicationDbContext db, Order order, CancellationToken cancellationToken)
    {
        var eventKey = $"order_delivered:{order.Id}";

        var exists = await db.FinancialEntries
            .AnyAsync(x => x.UserId == order.UserId && x.EventKey == eventKey, cancellationToken);

        if (exists)
            return;

        var description = order.IsQuickSale
            ? $"Venda rápida #{order.Number}"
            : $"Venda - Pedido #{order.Number}";

        db.FinancialEntries.Add(FinancialEntry.Create(
            order.UserId,
            FinancialEntryType.Income,
            decimal.Round(order.CalculateTotal(), 2, MidpointRounding.AwayFromZero),
            description,
            FinancialOriginType.Order,
            order.Id,
            eventKey));
    }
}
