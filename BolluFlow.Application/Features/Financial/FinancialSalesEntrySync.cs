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

        var sourceOrderIds = deliveredOrders.Select(x => x.Id).ToList();

        var existingOrderIds = await db.FinancialEntries
            .Where(x => x.UserId == userId && x.SourceOrderId != null && sourceOrderIds.Contains(x.SourceOrderId.Value))
            .Select(x => x.SourceOrderId!.Value)
            .ToListAsync(cancellationToken);

        var missingEntries = deliveredOrders
            .Where(x => !existingOrderIds.Contains(x.Id))
            .Select(x => FinancialEntry.Create(
                userId,
                $"Venda pedido #{x.Number} - {x.Customer.Name}",
                FinancialEntryType.Income,
                "Vendas",
                x.Items.Sum(i => i.Quantity * i.UnitPrice),
                DateOnly.FromDateTime(x.DeliveryDate?.Date ?? x.CreatedAt.Date),
                FinancialEntryStatus.Paid,
                false,
                "Lançamento automático de venda",
                true,
                x.Id))
            .ToList();

        if (missingEntries.Count == 0)
            return;

        db.FinancialEntries.AddRange(missingEntries);
        await db.SaveChangesAsync(cancellationToken);
    }
}
