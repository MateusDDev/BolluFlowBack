using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Ingredients.DeleteIngredient;

public class DeleteIngredientHandler(ApplicationDbContext db) : IRequestHandler<DeleteIngredientCommand, Result>
{
    public async Task<Result> Handle(DeleteIngredientCommand request, CancellationToken cancellationToken)
    {
        var ingredient = await db.Ingredients
            .FirstOrDefaultAsync(x => x.Id == request.IngredientId, cancellationToken);

        if (ingredient is null)
            return Result.NotFound(CommonErrors.IngredientNotFound);
        
        var hasRecipe = await db.RecipeIngredients
            .AnyAsync(x => x.IngredientId == ingredient.Id, cancellationToken);

        if (hasRecipe)
            return Result.BadRequest(new DomainError(
                "INGREDIENT_IN_USE", "Não é possivel excluir um ingrediente usado em uma receita"));
        
        db.Ingredients.Remove(ingredient);
        
        await db.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}