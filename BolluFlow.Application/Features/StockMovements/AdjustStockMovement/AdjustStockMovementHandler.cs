using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.StockMovements.AdjustStockMovement;

public class AdjustStockMovementHandler(ApplicationDbContext db)
    : IRequestHandler<AdjustStockMovementCommand, ResultOf<StockAdjustmentModel>>
{
    public async Task<ResultOf<StockAdjustmentModel>> Handle(AdjustStockMovementCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<StockItemType>(request.ItemType, true, out var itemType))
            return ResultOf<StockAdjustmentModel>.BadRequest(new("STOCK_ITEM_TYPE_INVALID", "itemType must be supply or ingredient"));

        if (request.Quantity <= 0)
            return ResultOf<StockAdjustmentModel>.BadRequest(new("STOCK_QUANTITY_INVALID", "quantity must be greater than zero"));

        if (request.UnitCost < 0)
            return ResultOf<StockAdjustmentModel>.BadRequest(new("STOCK_UNIT_COST_INVALID", "unitCost must be greater than or equal to zero"));

        if (string.IsNullOrWhiteSpace(request.Reason) || request.Reason.Trim().Length < 3)
            return ResultOf<StockAdjustmentModel>.BadRequest(new("STOCK_ADJUSTMENT_REASON_INVALID", "reason must have at least 3 characters"));

        var direction = request.Direction?.Trim().ToLowerInvariant();
        if (direction is not ("add" or "remove"))
            return ResultOf<StockAdjustmentModel>.BadRequest(new("STOCK_ADJUSTMENT_DIRECTION_INVALID", "direction must be add or remove"));

        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            string itemName;

            if (itemType == StockItemType.Supply)
            {
                var supply = await db.Supplies.FirstOrDefaultAsync(x => x.Id == request.ItemId, cancellationToken);
                if (supply is null)
                    return ResultOf<StockAdjustmentModel>.NotFound(new("SUPPLY_NOT_FOUND", "Supply not found"));

                itemName = supply.Name;

                if (direction == "add")
                    supply.RevertStock(request.Quantity);
                else
                    supply.ConsumeStock(request.Quantity);
            }
            else
            {
                var ingredient = await db.Ingredients.FirstOrDefaultAsync(x => x.Id == request.ItemId, cancellationToken);
                if (ingredient is null)
                    return ResultOf<StockAdjustmentModel>.NotFound(new("INGREDIENT_NOT_FOUND", "Ingredient not found"));

                itemName = ingredient.Name;

                if (direction == "add")
                    ingredient.RevertStock(request.Quantity);
                else
                    ingredient.ConsumeStock(request.Quantity);
            }

            var movement = StockMovement.Create(
                itemType,
                request.ItemId,
                StockMovementType.Adjustment,
                request.Quantity,
                request.UnitCost,
                request.Reason.Trim(),
                null);

            db.StockMovements.Add(movement);

            var isAdd = direction == "add";
            var entry = FinancialEntry.Create(
                isAdd ? FinancialEntryType.Expense : FinancialEntryType.Income,
                movement.TotalValue,
                isAdd
                    ? $"Ajuste de estoque (adição) - {itemName}"
                    : $"Ajuste de estoque (remoção) - {itemName}",
                FinancialOriginType.StockAdjustment,
                request.ItemId,
                null);

            db.FinancialEntries.Add(entry);

            await db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return ResultOf<StockAdjustmentModel>.Success(new StockAdjustmentModel
            {
                Id = movement.Id,
                ItemType = itemType.ToString().ToLowerInvariant(),
                ItemId = request.ItemId,
                MovementType = "adjustment",
                Quantity = movement.Quantity,
                UnitCost = movement.UnitCost,
                TotalValue = movement.TotalValue,
                Direction = direction,
                Reason = request.Reason.Trim(),
                CreatedAt = movement.CreatedAt
            });
        }
        catch (DomainException e)
        {
            await tx.RollbackAsync(cancellationToken);
            return ResultOf<StockAdjustmentModel>.BadRequest(e.Error);
        }
    }
}
