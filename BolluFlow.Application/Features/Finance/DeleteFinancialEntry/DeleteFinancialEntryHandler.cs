using BolluFlow.Application.Abstractions;
using BolluFlow.Domain.Abstractions;
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

        try
        {
            var movementId = TryGetStockMovementId(entry.EventKey);

            if (movementId is not null)
            {
                var movement = await db.StockMovements.FirstOrDefaultAsync(x => x.Id == movementId.Value, cancellationToken);

                if (movement is not null)
                {
                    if (movement.Quantity <= 0)
                        return Result.BadRequest(new DomainError("STOCK_REVERT_INVALID_QUANTITY", "Não foi possível reverter estoque: quantidade da movimentação inválida."));

                    if (movement.ItemType == StockItemType.Ingredient)
                    {
                        var ingredient = await db.Ingredients.FirstOrDefaultAsync(x => x.Id == movement.ItemId, cancellationToken);
                        if (ingredient is null)
                            return Result.BadRequest(new DomainError("INGREDIENT_NOT_FOUND", "Ingrediente relacionado à movimentação não encontrado."));

                        ingredient.ConsumeStock(movement.Quantity);
                    }
                    else
                    {
                        var supply = await db.Supplies.FirstOrDefaultAsync(x => x.Id == movement.ItemId, cancellationToken);
                        if (supply is null)
                            return Result.BadRequest(new DomainError("SUPPLY_NOT_FOUND", "Insumo relacionado à movimentação não encontrado."));

                        supply.ConsumeStock(movement.Quantity);
                    }

                    db.StockMovements.Remove(movement);
                }
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

    private static long? TryGetStockMovementId(string? eventKey)
    {
        const string prefix = "stock_purchase:";

        if (string.IsNullOrWhiteSpace(eventKey) || !eventKey.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            return null;

        var idPart = eventKey[prefix.Length..];

        return long.TryParse(idPart, out var movementId)
            ? movementId
            : null;
    }
}
