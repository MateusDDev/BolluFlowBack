using BolluFlow.Application.Abstractions;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Recipes.RecipeIngredients.GetRecipeIngredients;

public class GetRecipeIngredientsHandler(ApplicationDbContext db)
    : IRequestHandler<GetRecipeIngredientsQuery, ResultOf<List<GetRecipeIngredientsResult>>>
{
    public async Task<ResultOf<List<GetRecipeIngredientsResult>>> Handle(GetRecipeIngredientsQuery request, CancellationToken cancellationToken)
    {
        var recipeIngredients = await db.RecipeIngredients
            .Where(x => request.RecipeId == x.RecipeId)
            .Select(x => new GetRecipeIngredientsResult
            {
                RecipeId = x.RecipeId,
                IngredientId = x.IngredientId,
                Name = x.Ingredient.Name,
                Unit = x.Ingredient.Unit,
                MinimumStockQuantity = x.Ingredient.MinimumStockQuantity,
                StockQuantity = x.Ingredient.StockQuantity,
                UnitPrice = x.Ingredient.UnitPrice,
                QuantityUsed = x.QuantityUsed
            }).ToListAsync(cancellationToken);
        
        return ResultOf<List<GetRecipeIngredientsResult>>.Success(recipeIngredients);
    }
}