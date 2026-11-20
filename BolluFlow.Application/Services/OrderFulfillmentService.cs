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
        StockPolicy policy)
    {
        return order.Items
            .Select(item => new { Item = item, Product = products.GetValueOrDefault(item.ProductId) })
            .Where(x => x.Product is null || !policy.HasEnough(x.Product, x.Item.Quantity))
            .Select(x => new InsufficientProductStock(
                x.Item.ProductId,
                x.Product?.Name ?? "Produto não encontrado",
                x.Item.Quantity,
                x.Product?.StockQuantity ?? 0))
            .ToList();
    }

    public static void ConsumeStock(Order order, Dictionary<long, Product> products, StockPolicy policy)
    {
        foreach (var item in order.Items)
        {
            if (policy.Consume(products[item.ProductId], item.Quantity))
                item.MarkStockDebited();
        }
    }

    public static void RevertStock(Order order, Dictionary<long, Product> products)
    {
        foreach (var item in order.Items.Where(x => x.StockDebited))
        {
            if (products.TryGetValue(item.ProductId, out var product))
                product.RevertStock(item.Quantity);

            item.ClearStockDebited();
        }
    }

    public static async Task RemoveIncomeAsync(ApplicationDbContext db, Order order, CancellationToken cancellationToken)
    {
        var eventKey = IncomeEventKey(order);

        var entries = await db.FinancialEntries
            .Where(x => x.UserId == order.UserId && x.EventKey == eventKey)
            .ToListAsync(cancellationToken);

        db.FinancialEntries.RemoveRange(entries);
    }

    public static async Task RegisterIncomeAsync(ApplicationDbContext db, Order order, CancellationToken cancellationToken)
    {
        var eventKey = IncomeEventKey(order);

        var exists = await db.FinancialEntries
            .AnyAsync(x => x.UserId == order.UserId && x.EventKey == eventKey, cancellationToken);

        if (exists)
            return;

        var description = order.IsDirectSale
            ? $"Venda direta #{order.Number}"
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

    private static string IncomeEventKey(Order order) => $"order_delivered:{order.Id}";
}
