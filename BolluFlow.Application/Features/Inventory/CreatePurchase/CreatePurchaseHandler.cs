using System.Text.Json;
using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Features.Finance.Shared;
using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;
using BolluFlow.Domain.Errors;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Inventory.CreatePurchase;

public class CreatePurchaseHandler(ApplicationDbContext db) : IRequestHandler<CreatePurchaseCommand, ResultOf<object>>
{
    public async Task<ResultOf<object>> Handle(CreatePurchaseCommand request, CancellationToken cancellationToken)
    {
        if (request.TotalCost <= 0 || request.Quantity <= 0 || request.QuantityInBaseUnit <= 0)
            return ResultOf<object>.BadRequest(FinanceErrors.InvalidAmount);
        if (!FinanceMonth.TryParseDate(request.PurchasedAt, out var purchasedAt))
            return ResultOf<object>.BadRequest(FinanceErrors.InvalidDate);

        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        Purchase purchase;
        FinancialTransaction transaction;

        if (string.Equals(request.ItemType, "ingredient", StringComparison.OrdinalIgnoreCase))
        {
            var ingredient = await db.Ingredients.FirstOrDefaultAsync(x => x.Id == request.ItemId, cancellationToken);
            if (ingredient is null)
                return ResultOf<object>.NotFound(CommonErrors.IngredientNotFound);

            ingredient.ReplenishStock(request.QuantityInBaseUnit);
            purchase = Purchase.Create(PurchaseItemType.Ingredient, ingredient.Id, ingredient.Name, request.ConversionName, request.Quantity, request.QuantityInBaseUnit, request.TotalCost, purchasedAt, request.Notes);
            transaction = FinancialTransaction.Create(FinancialTransactionType.Expense, FinancialTransactionSourceType.IngredientPurchase, $"Ingredient purchase - {ingredient.Name}", request.TotalCost, purchasedAt, FinancialSourceEntityType.Purchase, purchase.Id, true, request.Notes, JsonSerializer.Serialize(new { conversionName = request.ConversionName, quantityInBaseUnit = request.QuantityInBaseUnit, label = "Stock replenishment" }));
        }
        else if (string.Equals(request.ItemType, "supply", StringComparison.OrdinalIgnoreCase))
        {
            var supply = await db.Supplies.FirstOrDefaultAsync(x => x.Id == request.ItemId, cancellationToken);
            if (supply is null)
                return ResultOf<object>.NotFound(CommonErrors.SupplyNotFound);

            supply.ReplenishStock(request.QuantityInBaseUnit);
            purchase = Purchase.Create(PurchaseItemType.Supply, supply.Id, supply.Name, request.ConversionName, request.Quantity, request.QuantityInBaseUnit, request.TotalCost, purchasedAt, request.Notes);
            transaction = FinancialTransaction.Create(FinancialTransactionType.Expense, FinancialTransactionSourceType.SupplyPurchase, $"Supply purchase - {supply.Name}", request.TotalCost, purchasedAt, FinancialSourceEntityType.Purchase, purchase.Id, true, request.Notes, JsonSerializer.Serialize(new { conversionName = request.ConversionName, quantityInBaseUnit = request.QuantityInBaseUnit, label = "Stock replenishment" }));
        }
        else
        {
            return ResultOf<object>.BadRequest(FinanceErrors.InvalidItemType);
        }

        db.Purchases.Add(purchase);
        db.FinancialTransactions.Add(transaction);
        await db.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);

        return ResultOf<object>.Success(new
        {
            purchase = new
            {
                id = purchase.Id,
                itemType = purchase.ItemType.ToString().ToLowerInvariant(),
                itemId = purchase.ItemId,
                itemName = purchase.ItemNameSnapshot,
                conversionName = purchase.ConversionNameSnapshot,
                quantity = purchase.Quantity,
                quantityInBaseUnit = purchase.QuantityInBaseUnit,
                totalCost = purchase.TotalCost,
                purchasedAt = purchase.PurchasedAt.ToString("yyyy-MM-dd"),
                notes = purchase.Notes
            },
            transaction = transaction.ToDto()
        });
    }
}
