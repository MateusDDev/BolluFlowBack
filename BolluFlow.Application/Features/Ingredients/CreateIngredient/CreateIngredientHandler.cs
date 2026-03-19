using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Financial;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;

namespace BolluFlow.Application.Features.Ingredients.CreateIngredient;

public class CreateIngredientHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<CreateIngredientCommand, ResultOf<CreateIngredientResult>>
{
    public async Task<ResultOf<CreateIngredientResult>> Handle(CreateIngredientCommand request, CancellationToken cancellationToken)
    {
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

            await FinancialStockPurchaseSync.RegisterIngredientPurchaseAsync(
                db,
                user.Id,
                ingredient,
                request.StockQuantity,
                request.UnitCost,
                cancellationToken);

            return ResultOf<CreateIngredientResult>.Success(new CreateIngredientResult(ingredient.Id));
        }
        catch (DomainException e)
        {
            return ResultOf<CreateIngredientResult>.BadRequest(e.Error);
        }
    }
}
