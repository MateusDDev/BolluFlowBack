using BolluFlow.Application.Abstractions;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Ingredients.CreateIngredient;

public class CreateIngredientHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<CreateIngredientCommand, ResultOf<CreateIngredientResult>>
{
    public async Task<ResultOf<CreateIngredientResult>> Handle(CreateIngredientCommand request, CancellationToken cancellationToken)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var ingredient = Ingredient.Create(
                user.Id,
                request.Name,
                request.Unit,
                request.UnitCost,
                request.StockQuantity,
                request.MinimumStockQuantity);

            db.Ingredients.Add(ingredient);

            await db.SaveChangesAsync(cancellationToken);

            if (request.StockQuantity > 0)
            {
                var eventKey = $"initial_stock:ingredient:{ingredient.Id}";
                var existingMovement = await db.StockMovements.FirstOrDefaultAsync(x => x.EventKey == eventKey, cancellationToken);

                if (existingMovement is null)
                {
                    var movement = StockMovement.Create(
                        StockItemType.Ingredient,
                        ingredient.Id,
                        StockMovementType.Purchase,
                        request.StockQuantity,
                        request.UnitCost,
                        "Estoque inicial",
                        eventKey);

                    db.StockMovements.Add(movement);

                    var existingEntry = await db.FinancialEntries.FirstOrDefaultAsync(x => x.EventKey == eventKey, cancellationToken);

                    if (existingEntry is null)
                    {
                        db.FinancialEntries.Add(FinancialEntry.Create(
                            FinancialEntryType.Expense,
                            movement.TotalValue,
                            $"Estoque inicial - {ingredient.Name}",
                            FinancialOriginType.IngredientPurchase,
                            ingredient.Id,
                            eventKey));
                    }
                }
            }

            await db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return ResultOf<CreateIngredientResult>.Success(new CreateIngredientResult(ingredient.Id));
        }
        catch (DomainException e)
        {
            await tx.RollbackAsync(cancellationToken);
            return ResultOf<CreateIngredientResult>.BadRequest(e.Error);
        }
    }
}
