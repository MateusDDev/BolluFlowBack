using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Financial;

public static class FinancialStockPurchaseSync
{
    public static async Task EnsureFinancialEntries(ApplicationDbContext db, long userId, CancellationToken cancellationToken)
    {
        var movementIds = await db.InventoryPurchaseMovements
            .Where(x => x.UserId == userId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (movementIds.Count == 0)
            return;

        var existingReferenceIds = await db.FinancialEntries
            .Where(x => x.UserId == userId && x.Source == FinancialEntrySource.AutoStockPurchase && x.ReferenceId != null && movementIds.Contains(x.ReferenceId.Value))
            .Select(x => x.ReferenceId!.Value)
            .ToListAsync(cancellationToken);

        var movementsWithoutEntry = await db.InventoryPurchaseMovements
            .Where(x => x.UserId == userId && !existingReferenceIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (movementsWithoutEntry.Count == 0)
            return;

        var entries = movementsWithoutEntry.Select(x => FinancialEntry.CreateAutomatic(
            userId,
            x.Description,
            FinancialEntryType.Expense,
            x.Category,
            x.TotalAmount,
            x.Date,
            FinancialEntryStatus.Paid,
            false,
            "Lançamento automático de compra de estoque",
            FinancialEntrySource.AutoStockPurchase,
            x.Id));

        db.FinancialEntries.AddRange(entries);
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
            FinancialEntrySource.AutoStockPurchase,
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
            FinancialEntrySource.AutoStockPurchase,
            movement.Id));

        await db.SaveChangesAsync(cancellationToken);
    }
}
