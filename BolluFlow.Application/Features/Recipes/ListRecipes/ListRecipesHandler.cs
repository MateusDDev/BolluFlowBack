using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Recipes.ListRecipes;

public class ListRecipesHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<ListRecipesQuery, ResultOf<List<RecipeModel>>>
{
    public async Task<ResultOf<List<RecipeModel>>> Handle(ListRecipesQuery request, CancellationToken cancellationToken)
    {
        var recipes = await db.Recipes
            .AsNoTracking()
            .Include(x => x.RecipeIngredients)
            .ThenInclude(x => x.Ingredient)
            .Where(x => x.UserId == user.Id)
            .OrderByDescending(x => x.Id)
            .Select(x => new RecipeModel
            {
                Id = x.Id,
                Name = x.Name,
                Category = x.Category,
                BaseUnit = x.BaseUnit,
                PreparationTime = x.PreparationTime,
                Yield = x.Yield,
                UnitCost = x.CalculateUnitCost(),
                CostPerBaseUnit = x.CostPerBaseUnit,
                FinalPrice = x.FinalPrice,
                StockQuantity = x.StockQuantity,
                MinimumStockQuantity = x.MinimumStockQuantity,
                IsActive = x.IsActive,
                TrackStock = x.TrackStock
            })
            .ToListAsync(cancellationToken);

        return ResultOf<List<RecipeModel>>.Success(recipes);
    }
}
