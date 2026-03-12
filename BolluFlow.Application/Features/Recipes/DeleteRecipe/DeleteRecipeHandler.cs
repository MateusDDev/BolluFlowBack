using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Recipes.DeleteRecipe;

public class DeleteRecipeHandler(ApplicationDbContext db) : IRequestHandler<DeleteRecipeCommand, Result>
{
    public async Task<Result> Handle(DeleteRecipeCommand request, CancellationToken cancellationToken)
    {
        var recipe = await db.Recipes
            .FirstOrDefaultAsync(x => x.Id == request.RecipeId ,cancellationToken);

        if (recipe is null)
            return Result.BadRequest(CommonErrors.RecipeNotFound);

        var linkedProductComponents = await db.ProductComponents
            .AnyAsync(x => x.RecipeId == recipe.Id, cancellationToken);

        if (linkedProductComponents)
        {
            return Result.Conflict(new DomainError("RECIPE_USED_BY_PRODUCT",
                "Não é possível excluir uma receita presente na composição de produto"));
        }

        var recipeItems = await db.RecipeIngredients
            .Where(x => x.RecipeId == recipe.Id)
            .ToListAsync(cancellationToken);
        
        db.RecipeIngredients.RemoveRange(recipeItems);
        db.Recipes.Remove(recipe);
        
        await db.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}
