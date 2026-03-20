using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Financial;

public static class FinancialSalesEntrySync
{
    public static async Task EnsureSalesEntries(ApplicationDbContext db, long userId, CancellationToken cancellationToken)
    {
        var deliveredOrders = await db.Orders
            .Include(x => x.Customer)
            .Include(x => x.Items)
            .Where(x => x.Customer.UserId == userId && x.Status == OrderStatus.Delivered)
            .ToListAsync(cancellationToken);

        if (deliveredOrders.Count == 0)
            return;

        var existingKeys = await db.FinancialEntries
            .Where(x => x.UserId == userId && x.Source == FinancialEntrySource.AutoSale && x.ReferenceId != null)
            .Select(x => new { x.ReferenceId, x.Date })
            .ToListAsync(cancellationToken);

        var missingEntries = deliveredOrders
            .Where(order => !existingKeys.Any(existing => existing.ReferenceId == order.Id && existing.Date == DateOnly.FromDateTime((order.DeliveryDate ?? order.CreatedAt).Date)))
            .Select(order => FinancialEntry.CreateAutomatic(
                userId,
                $"Venda pedido #{order.Number} - {order.Customer.Name}",
                FinancialEntryType.Income,
                "Vendas",
                decimal.Round(order.Items.Sum(i => i.Quantity * i.UnitPrice), 2, MidpointRounding.AwayFromZero),
                DateOnly.FromDateTime((order.DeliveryDate ?? order.CreatedAt).Date),
                FinancialEntryStatus.Paid,
                false,
                "Lançamento automático de venda",
                FinancialEntrySource.AutoSale,
                FinancialReferenceType.Order,
                order.Id))
            .ToList();

        if (missingEntries.Count == 0)
            return;

        db.FinancialEntries.AddRange(missingEntries);
        await db.SaveChangesAsync(cancellationToken);
    }
}
