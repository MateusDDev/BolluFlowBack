using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Financial;

public static class FinancialStockPurchaseSync
{
    public static async Task EnsureFinancialEntries(ApplicationDbContext db, long userId, CancellationToken cancellationToken)
    {
        var movements = await db.InventoryPurchaseMovements
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);

        if (movements.Count == 0)
            return;

        var existingKeys = await db.FinancialEntries
            .Where(x => x.UserId == userId && x.Source == FinancialEntrySource.AutoStock && x.ReferenceId != null)
            .Select(x => new { x.ReferenceId, x.Date })
            .ToListAsync(cancellationToken);

        var missingEntries = movements
            .Where(movement => !existingKeys.Any(existing => existing.ReferenceId == movement.Id && existing.Date == movement.Date))
            .Select(movement => FinancialEntry.CreateAutomatic(
                userId,
                movement.Description,
                FinancialEntryType.Expense,
                movement.Category,
                movement.TotalAmount,
                movement.Date,
                FinancialEntryStatus.Paid,
                false,
                "Lançamento automático de compra de estoque",
                FinancialEntrySource.AutoStock,
                FinancialReferenceType.StockPurchase,
                movement.Id))
            .ToList();

        if (missingEntries.Count == 0)
            return;

        db.FinancialEntries.AddRange(missingEntries);
        await db.SaveChangesAsync(cancellationToken);
    }

    public static async Task RegisterIngredientPurchaseAsync(ApplicationDbContext db, long userId, Ingredient ingredient, decimal quantityAdded, decimal unitCost, CancellationToken cancellationToken)
    {
        if (quantityAdded <= 0)
            return;

        var movement = InventoryPurchaseMovement.Create(
            userId,
            InventoryPurchaseItemType.Ingredient,
            ingredient.Id,
            ingredient.Name,
            quantityAdded,
            unitCost,
            DateOnly.FromDateTime(DateTime.UtcNow.Date),
            "Ingredientes",
            $"Compra de ingrediente: {ingredient.Name}");

        db.InventoryPurchaseMovements.Add(movement);
        await db.SaveChangesAsync(cancellationToken);

        db.FinancialEntries.Add(FinancialEntry.CreateAutomatic(
            userId,
            movement.Description,
            FinancialEntryType.Expense,
            movement.Category,
            movement.TotalAmount,
            movement.Date,
            FinancialEntryStatus.Paid,
            false,
            "Lançamento automático de compra de estoque",
            FinancialEntrySource.AutoStock,
            FinancialReferenceType.StockPurchase,
            movement.Id));

        await db.SaveChangesAsync(cancellationToken);
    }

    public static async Task RegisterSupplyPurchaseAsync(ApplicationDbContext db, long userId, Supply supply, decimal quantityAdded, decimal unitCost, CancellationToken cancellationToken)
    {
        if (quantityAdded <= 0)
            return;

        var movement = InventoryPurchaseMovement.Create(
            userId,
            InventoryPurchaseItemType.Supply,
            supply.Id,
            supply.Name,
            quantityAdded,
            unitCost,
            DateOnly.FromDateTime(DateTime.UtcNow.Date),
            "Insumos",
            $"Compra de insumo: {supply.Name}");

        db.InventoryPurchaseMovements.Add(movement);
        await db.SaveChangesAsync(cancellationToken);

        db.FinancialEntries.Add(FinancialEntry.CreateAutomatic(
            userId,
            movement.Description,
            FinancialEntryType.Expense,
            movement.Category,
            movement.TotalAmount,
            movement.Date,
            FinancialEntryStatus.Paid,
            false,
            "Lançamento automático de compra de estoque",
            FinancialEntrySource.AutoStock,
            FinancialReferenceType.StockPurchase,
            movement.Id));

        await db.SaveChangesAsync(cancellationToken);
    }
}
