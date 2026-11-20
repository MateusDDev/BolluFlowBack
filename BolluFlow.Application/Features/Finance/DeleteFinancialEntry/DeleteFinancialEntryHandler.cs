using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Services;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Finance.DeleteFinancialEntry;

public class DeleteFinancialEntryHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<DeleteFinancialEntryCommand, Result>
{
    public async Task<Result> Handle(DeleteFinancialEntryCommand request, CancellationToken cancellationToken)
    {
        var entry = await db.FinancialEntries
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == user.Id, cancellationToken);

        if (entry is null)
            return Result.NotFound(new DomainError("FINANCIAL_ENTRY_NOT_FOUND", "Lançamento financeiro não encontrado"));

        var isStockPurchase = entry.Origin is FinancialOriginType.IngredientPurchase or FinancialOriginType.SupplyPurchase;

        if (!request.RevertStock || !isStockPurchase)
        {
            db.FinancialEntries.Remove(entry);
            await db.SaveChangesAsync(cancellationToken);
            return Result.NoContent();
        }

        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        var policy = await StockPolicy.LoadAsync(db, user.Id, cancellationToken);

        try
        {
            var movement = await FindStockMovementAsync(entry.EventKey, cancellationToken);

            if (movement is not null)
            {
                if (movement.Quantity <= 0)
                    return Result.BadRequest(new DomainError("STOCK_REVERT_INVALID_QUANTITY", "Não foi possível reverter estoque: quantidade da movimentação inválida."));

                if (movement.ItemType == StockItemType.Ingredient)
                {
                    var ingredient = await db.Ingredients.FirstOrDefaultAsync(x => x.Id == movement.ItemId && x.UserId == user.Id, cancellationToken);
                    if (ingredient is null)
                        return Result.BadRequest(new DomainError("INGREDIENT_NOT_FOUND", "Ingrediente relacionado à movimentação não encontrado."));

                    policy.Consume(ingredient, movement.Quantity);

                    if (await ShouldRestoreCostAsync(movement, cancellationToken))
                        ingredient.UpdateUnitCost(movement.PreviousUnitCost!.Value);
                }
                else
                {
                    var supply = await db.Supplies.FirstOrDefaultAsync(x => x.Id == movement.ItemId && x.UserId == user.Id, cancellationToken);
                    if (supply is null)
                        return Result.BadRequest(new DomainError("SUPPLY_NOT_FOUND", "Insumo relacionado à movimentação não encontrado."));

                    policy.Consume(supply, movement.Quantity);

                    if (await ShouldRestoreCostAsync(movement, cancellationToken))
                        supply.UpdateUnitCost(movement.PreviousUnitCost!.Value);
                }

                db.StockMovements.Remove(movement);
            }

            db.FinancialEntries.Remove(entry);
            await db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return Result.NoContent();
        }
        catch (DomainException e)
        {
            await tx.RollbackAsync(cancellationToken);

            return Result.BadRequest(new DomainError(
                "STOCK_REVERT_FAILED",
                $"Não foi possível reverter estoque sem invalidar o saldo: {e.Error.Message}"));
        }
    }

    private async Task<StockMovement?> FindStockMovementAsync(string? eventKey, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(eventKey))
            return null;

        const string purchasePrefix = "stock_purchase:";

        if (eventKey.StartsWith(purchasePrefix, StringComparison.OrdinalIgnoreCase))
        {
            return long.TryParse(eventKey[purchasePrefix.Length..], out var movementId)
                ? await db.StockMovements.FirstOrDefaultAsync(x => x.Id == movementId, cancellationToken)
                : null;
        }

        return await db.StockMovements.FirstOrDefaultAsync(x => x.EventKey == eventKey, cancellationToken);
    }

    private async Task<bool> ShouldRestoreCostAsync(StockMovement movement, CancellationToken cancellationToken)
    {
        if (movement.PreviousUnitCost is not > 0)
            return false;

        var hasLaterCostUpdate = await db.StockMovements.AnyAsync(x =>
            x.ItemType == movement.ItemType &&
            x.ItemId == movement.ItemId &&
            x.Id > movement.Id &&
            x.PreviousUnitCost != null, cancellationToken);

        return !hasLaterCostUpdate;
    }
}
