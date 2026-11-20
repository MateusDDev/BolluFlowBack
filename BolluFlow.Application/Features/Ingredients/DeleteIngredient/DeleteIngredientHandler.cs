using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Ingredients.DeleteIngredient;

public class DeleteIngredientHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<DeleteIngredientCommand, Result>
{
    public async Task<Result> Handle(DeleteIngredientCommand request, CancellationToken cancellationToken)
    {
        var ingredient = await db.Ingredients
            .FirstOrDefaultAsync(x => x.Id == request.IngredientId && x.UserId == user.Id, cancellationToken);

        if (ingredient is null)
            return Result.NotFound(CommonErrors.IngredientNotFound);

        var inUse = await db.RecipeIngredients.AnyAsync(x => x.IngredientId == ingredient.Id, cancellationToken)
                    || await db.ProductComponents.AnyAsync(x => x.IngredientId == ingredient.Id, cancellationToken);

        if (inUse)
            return Result.BadRequest(new DomainError(
                "INGREDIENT_IN_USE", "Este ingrediente está em uma receita ou ficha técnica. Remova de lá antes de excluir"));

        db.Ingredients.Remove(ingredient);

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
