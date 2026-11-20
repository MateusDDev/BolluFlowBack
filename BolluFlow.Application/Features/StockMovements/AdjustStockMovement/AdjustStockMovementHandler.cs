using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Application.Services;
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
            return ResultOf<StockAdjustmentModel>.BadRequest(new("STOCK_QUANTITY_INVALID", "A quantidade deve ser maior que zero."));

        if (request.UnitCost < 0)
            return ResultOf<StockAdjustmentModel>.BadRequest(new("STOCK_UNIT_COST_INVALID", "O custo não pode ser negativo."));

        if (string.IsNullOrWhiteSpace(request.Reason) || request.Reason.Trim().Length < 3)
            return ResultOf<StockAdjustmentModel>.BadRequest(new("STOCK_ADJUSTMENT_REASON_INVALID", "Informe o motivo do ajuste (mínimo de 3 caracteres)."));

        var policy = await StockPolicy.LoadAsync(db, user.Id, cancellationToken);

        if (!policy.IsActive)
            return ResultOf<StockAdjustmentModel>.Unprocessable(new("STOCK_MODULE_DISABLED", "Ative o módulo Estoque para ajustar quantidades."));

        try
        {
            if (request.ItemType == StockItemType.Supply)
            {
                var supply = await db.Supplies.FirstOrDefaultAsync(x => x.Id == request.ItemId && x.UserId == user.Id, cancellationToken);
                if (supply is null)
                    return ResultOf<StockAdjustmentModel>.NotFound(new("SUPPLY_NOT_FOUND", "Insumo não encontrado."));

                supply.SetTrackStock(true);

                if (request.Direction == AdjustmentDirection.Add)
                    supply.AddStock(request.Quantity);
                else
                    policy.Consume(supply, request.Quantity);
            }
            else
            {
                var ingredient = await db.Ingredients.FirstOrDefaultAsync(x => x.Id == request.ItemId && x.UserId == user.Id, cancellationToken);
                if (ingredient is null)
                    return ResultOf<StockAdjustmentModel>.NotFound(new("INGREDIENT_NOT_FOUND", "Ingrediente não encontrado."));

                ingredient.SetTrackStock(true);

                if (request.Direction == AdjustmentDirection.Add)
                    ingredient.AddStock(request.Quantity);
                else
                    policy.Consume(ingredient, request.Quantity);
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
            await db.SaveChangesAsync(cancellationToken);

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
            return ResultOf<StockAdjustmentModel>.BadRequest(e.Error);
        }
    }
}
