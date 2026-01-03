using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Recipes.RecipeIngredients.UpdateRecipeIngredient;

public class UpdateRecipeIngredientHandler(ApplicationDbContext db) : IRequestHandler<UpdateRecipeIngredientCommand, Result>
{
    public async Task<Result> Handle(UpdateRecipeIngredientCommand request, CancellationToken cancellationToken)
    {
        var command = request.Body;
        
        var recipeIngredient = await db.RecipeIngredients
            .FirstOrDefaultAsync(x => x.RecipeId == request.RecipeId && x.IngredientId == request.IngredientId,
                cancellationToken);

        if (recipeIngredient is null)
            return Result.NotFound(CommonErrors.RecipeIngredientNotFound);
        
        recipeIngredient.QuantityUsed = command.QuantityUsed;
        
        await db.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}