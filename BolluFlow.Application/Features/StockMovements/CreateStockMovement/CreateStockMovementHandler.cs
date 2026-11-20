using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Application.Services;
using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.StockMovements.CreateStockMovement;

public class CreateStockMovementHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<CreateStockMovementCommand, ResultOf<StockMovementModel>>
{
    public async Task<ResultOf<StockMovementModel>> Handle(CreateStockMovementCommand request, CancellationToken cancellationToken)
    {

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

        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        var movement = StockMovement.Create(
            request.ItemType,
            request.ItemId,
            request.MovementType,
            request.Quantity,
            request.UnitCost,
            request.Notes,
            request.EventKey);

        db.StockMovements.Add(movement);
        await db.SaveChangesAsync(cancellationToken);

        if (request.MovementType == StockMovementType.Purchase)
        {
            var itemName = $"#{request.ItemId}";

            var policy = await StockPolicy.LoadAsync(db, user.Id, cancellationToken);

            if (request.ItemType == StockItemType.Ingredient)
            {
                var ingredient = await db.Ingredients
                    .FirstOrDefaultAsync(x => x.Id == request.ItemId && x.UserId == user.Id, cancellationToken);

                if (ingredient is not null)
                {
                    if (policy.IsActive)
                        ingredient.AddStock(movement.Quantity);

                    if (request.UpdateItemCost && request.UnitCost > 0)
                        ingredient.UpdateUnitCost(request.UnitCost);

                    itemName = ingredient.Name;
                }
            }
            else if (request.ItemType == StockItemType.Supply)
            {
                var supply = await db.Supplies
                    .FirstOrDefaultAsync(x => x.Id == request.ItemId && x.UserId == user.Id, cancellationToken);

                if (supply is not null)
                {
                    if (policy.IsActive)
                        supply.AddStock(movement.Quantity);

                    if (request.UpdateItemCost && request.UnitCost > 0)
                        supply.UpdateUnitCost(request.UnitCost);

                    itemName = supply.Name;
                }
            }

            await db.SaveChangesAsync(cancellationToken);

            var eventKey = $"stock_purchase:{movement.Id}";
            var existingEntry = await db.FinancialEntries.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == user.Id && x.EventKey == eventKey, cancellationToken);

            if (existingEntry is null)
            {
                var origin = request.ItemType == StockItemType.Ingredient
                    ? FinancialOriginType.IngredientPurchase
                    : FinancialOriginType.SupplyPurchase;

                var entry = FinancialEntry.Create(
                    user.Id,
                    FinancialEntryType.Expense,
                    movement.TotalValue,
                    $"Compra - {itemName}",
                    origin,
                    movement.ItemId,
                    eventKey);

                db.FinancialEntries.Add(entry);
                await db.SaveChangesAsync(cancellationToken);
            }
        }

        await tx.CommitAsync(cancellationToken);

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
