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
        
        var ordersItems = await db.OrderItems
            .Where(x => x.RecipeId == recipe.Id)
            .ToListAsync(cancellationToken);

        if (ordersItems.Count > 0)
        {
            return Result.BadRequest(new DomainError("RECIPE_IS_IN_AN_ORDER",
                "Não é possível excluir uma receita presente em uma ordem"));
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