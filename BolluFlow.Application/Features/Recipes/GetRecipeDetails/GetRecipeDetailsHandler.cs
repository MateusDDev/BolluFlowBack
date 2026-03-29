using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Recipes.GetRecipeDetails;

public class GetRecipeDetailsHandler(ApplicationDbContext db) : IRequestHandler<GetRecipeDetailsQuery, ResultOf<RecipeModel>>
{
    public async Task<ResultOf<RecipeModel>> Handle(GetRecipeDetailsQuery request, CancellationToken cancellationToken)
    {
        var recipe = await db.Recipes
            .Include(x => x.RecipeIngredients)
            .ThenInclude(x => x.Ingredient)
            .FirstOrDefaultAsync(x => x.Id == request.RecipeId, cancellationToken);

        if (recipe is null)
            return ResultOf<RecipeModel>.BadRequest(CommonErrors.RecipeNotFound);

        return ResultOf<RecipeModel>.Success(new RecipeModel
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Category = recipe.Category,
            BaseUnit = recipe.BaseUnit,
            PreparationTime = recipe.PreparationTime,
            UnitCost = recipe.CalculateUnitCost(),
            FinalPrice = recipe.FinalPrice,
            StockQuantity = recipe.StockQuantity,
            MinimumStockQuantity = recipe.MinimumStockQuantity,
            IsActive = recipe.IsActive
        });
    }
}
