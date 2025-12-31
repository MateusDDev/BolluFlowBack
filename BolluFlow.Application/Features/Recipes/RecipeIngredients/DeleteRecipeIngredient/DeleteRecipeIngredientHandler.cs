using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Recipes.RecipeIngredients.DeleteRecipeIngredient;

public class DeleteRecipeIngredientHandler(ApplicationDbContext db)
    : IRequestHandler<DeleteRecipeIngredientCommand, Result>
{
    public async Task<Result> Handle(DeleteRecipeIngredientCommand request, CancellationToken cancellationToken)
    {
        var recipeIngredient = await db.RecipeIngredients
            .FirstOrDefaultAsync(x => x.RecipeId == request.RecipeId && x.IngredientId == request.IngredientId,
                cancellationToken);

        if (recipeIngredient is null)
            return Result.NotFound(CommonErrors.RecipeIngredientNotFound);
        
        db.RecipeIngredients.Remove(recipeIngredient);
        
        await db.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}