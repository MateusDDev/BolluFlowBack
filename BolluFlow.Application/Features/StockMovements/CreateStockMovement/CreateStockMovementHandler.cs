using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.StockMovements.CreateStockMovement;

public class CreateStockMovementHandler(ApplicationDbContext db)
    : IRequestHandler<CreateStockMovementCommand, ResultOf<StockMovementModel>>
{
    public async Task<ResultOf<StockMovementModel>> Handle(CreateStockMovementCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<StockItemType>(request.ItemType, true, out var itemType))
            return ResultOf<StockMovementModel>.BadRequest(new("STOCK_ITEM_TYPE_INVALID", "item_type inválido"));

        if (!Enum.TryParse<StockMovementType>(request.MovementType, true, out var movementType))
            return ResultOf<StockMovementModel>.BadRequest(new("STOCK_MOVEMENT_TYPE_INVALID", "movement_type inválido"));

        if (request.Quantity <= 0)
            return ResultOf<StockMovementModel>.BadRequest(new("STOCK_QUANTITY_INVALID", "quantity deve ser maior que zero"));

        if (request.UnitCost < 0)
            return ResultOf<StockMovementModel>.BadRequest(new("STOCK_UNIT_COST_INVALID", "unit_cost deve ser maior ou igual a zero"));

        if (!string.IsNullOrWhiteSpace(request.EventKey))
        {
            var existingByEvent = await db.StockMovements
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.EventKey == request.EventKey, cancellationToken);

            if (existingByEvent is not null)
                return ResultOf<StockMovementModel>.Success(ToModel(existingByEvent));
        }

        var movement = StockMovement.Create(
            itemType,
            request.ItemId,
            movementType,
            request.Quantity,
            request.UnitCost,
            request.Notes,
            request.EventKey);

        db.StockMovements.Add(movement);
        await db.SaveChangesAsync(cancellationToken);

        if (movementType == StockMovementType.Purchase)
        {
            var eventKey = $"stock_purchase:{movement.Id}";
            var existingEntry = await db.FinancialEntries.AsNoTracking().FirstOrDefaultAsync(x => x.EventKey == eventKey, cancellationToken);

            if (existingEntry is null)
            {
                var origin = itemType == StockItemType.Ingredient
                    ? FinancialOriginType.IngredientPurchase
                    : FinancialOriginType.SupplyPurchase;

                var entry = FinancialEntry.Create(
                    FinancialEntryType.Expense,
                    movement.TotalValue,
                    $"Despesa automática de compra de {(itemType == StockItemType.Ingredient ? "ingrediente" : "insumo")} #{movement.ItemId}",
                    origin,
                    movement.ItemId,
                    eventKey);

                db.FinancialEntries.Add(entry);
                await db.SaveChangesAsync(cancellationToken);
            }
        }

        return ResultOf<StockMovementModel>.Success(ToModel(movement));
    }

    private static StockMovementModel ToModel(StockMovement movement) => new()
    {
        Id = movement.Id,
        ItemType = movement.ItemType.ToString().ToLowerInvariant(),
        ItemId = movement.ItemId,
        MovementType = movement.MovementType.ToString().ToLowerInvariant(),
        Quantity = movement.Quantity,
        UnitCost = movement.UnitCost,
        TotalValue = movement.TotalValue,
        Notes = movement.Notes,
        EventKey = movement.EventKey,
        CreatedAt = movement.CreatedAt
    };
}
