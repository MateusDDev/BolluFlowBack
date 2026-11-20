using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.StockMovements.AdjustStockMovement;

public class AdjustStockMovementHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<AdjustStockMovementCommand, ResultOf<StockAdjustmentModel>>
{
    public async Task<ResultOf<StockAdjustmentModel>> Handle(AdjustStockMovementCommand request, CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0)
            return ResultOf<StockAdjustmentModel>.BadRequest(new("STOCK_QUANTITY_INVALID", "quantity must be greater than zero"));

        if (request.UnitCost < 0)
            return ResultOf<StockAdjustmentModel>.BadRequest(new("STOCK_UNIT_COST_INVALID", "unitCost must be greater than or equal to zero"));

        if (string.IsNullOrWhiteSpace(request.Reason) || request.Reason.Trim().Length < 3)
            return ResultOf<StockAdjustmentModel>.BadRequest(new("STOCK_ADJUSTMENT_REASON_INVALID", "reason must have at least 3 characters"));

        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            string itemName;

            if (request.ItemType == StockItemType.Supply)
            {
                var supply = await db.Supplies.FirstOrDefaultAsync(x => x.Id == request.ItemId, cancellationToken);
                if (supply is null)
                    return ResultOf<StockAdjustmentModel>.NotFound(new("SUPPLY_NOT_FOUND", "Supply not found"));

                itemName = supply.Name;

                supply.SetTrackStock(true);

                if (request.Direction == AdjustmentDirection.Add)
                    supply.AddStock(request.Quantity);
                else
                    supply.ConsumeStock(request.Quantity);
            }
            else
            {
                var ingredient = await db.Ingredients.FirstOrDefaultAsync(x => x.Id == request.ItemId, cancellationToken);
                if (ingredient is null)
                    return ResultOf<StockAdjustmentModel>.NotFound(new("INGREDIENT_NOT_FOUND", "Ingredient not found"));

                itemName = ingredient.Name;

                ingredient.SetTrackStock(true);

                if (request.Direction == AdjustmentDirection.Add)
                    ingredient.AddStock(request.Quantity);
                else
                    ingredient.ConsumeStock(request.Quantity);
            }

            var movement = StockMovement.Create(
                request.ItemType,
                request.ItemId,
                StockMovementType.Adjustment,
                request.Quantity,
                request.UnitCost,
                request.Reason.Trim(),
                null);

            db.StockMovements.Add(movement);

            var isAdd = request.Direction == AdjustmentDirection.Add;
            var entry = FinancialEntry.Create(
                user.Id,
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
                ItemType = request.ItemType.ToString().ToLowerInvariant(),
                ItemId = request.ItemId,
                MovementType = "adjustment",
                Quantity = movement.Quantity,
                UnitCost = movement.UnitCost,
                TotalValue = movement.TotalValue,
                Direction = request.Direction.ToString().ToLowerInvariant(),
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
