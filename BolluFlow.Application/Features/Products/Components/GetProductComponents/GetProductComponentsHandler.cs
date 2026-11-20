using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Entities;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Products.Components.GetProductComponents;

public class GetProductComponentsHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<GetProductComponentsQuery, ResultOf<List<ProductComponentModel>>>
{
    public async Task<ResultOf<List<ProductComponentModel>>> Handle(GetProductComponentsQuery request, CancellationToken cancellationToken)
    {
        var components = await db.ProductComponents
            .Where(x => x.ProductId == request.ProductId && x.Product.UserId == user.Id)
            .ToListAsync(cancellationToken);

        var recipeIds = components
            .Where(x => x.RecipeId.HasValue)
            .Select(x => x.RecipeId!.Value)
            .Distinct()
            .ToList();

        var recipes = recipeIds.Count > 0
            ? await db.Recipes
                .Include(x => x.RecipeIngredients)
                .ThenInclude(x => x.Ingredient)
                .Where(x => recipeIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken)
            : new Dictionary<long, Recipe>();

        var ingredientIds = components
            .Where(x => x.IngredientId.HasValue)
            .Select(x => x.IngredientId!.Value)
            .Distinct()
            .ToList();

        var supplyIds = components
            .Where(x => x.SupplyId.HasValue)
            .Select(x => x.SupplyId!.Value)
            .Distinct()
            .ToList();

        var ingredients = ingredientIds.Count > 0
            ? await db.Ingredients
                .Where(x => ingredientIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken)
            : new Dictionary<long, Ingredient>();

        var supplies = supplyIds.Count > 0
            ? await db.Supplies
                .Where(x => supplyIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken)
            : new Dictionary<long, Supply>();

        var result = components.Select(x =>
        {
            decimal? costPerBaseUnit = null;
            decimal? yield = null;
            decimal stockQuantity = 0;
            var unitPrice = x.UnitPrice;

            if (x.RecipeId.HasValue && recipes.TryGetValue(x.RecipeId.Value, out var recipe))
            {
                stockQuantity = recipe.StockQuantity;
                yield = recipe.Yield;
                costPerBaseUnit = recipe.CostPerBaseUnit;
                unitPrice = recipe.CostPerBaseUnit ?? 0m;
            }
            else if (x.IngredientId.HasValue && ingredients.TryGetValue(x.IngredientId.Value, out var ingredient))
            {
                stockQuantity = ingredient.StockQuantity;
                unitPrice = ingredient.UnitCost;
            }
            else if (x.SupplyId.HasValue && supplies.TryGetValue(x.SupplyId.Value, out var supply))
            {
                stockQuantity = supply.StockQuantity;
                unitPrice = supply.UnitCost;
            }

            return new ProductComponentModel
            {
                Id = x.Id,
                ProductId = x.ProductId,
                ComponentType = x.ComponentType,
                RecipeId = x.RecipeId,
                IngredientId = x.IngredientId,
                SupplyId = x.SupplyId,
                Name = x.Name,
                BaseUnit = x.BaseUnit,
                UnitPrice = unitPrice,
                QuantityUsed = x.QuantityUsed,
                StockQuantity = stockQuantity,
                Yield = yield,
                CostPerBaseUnit = costPerBaseUnit
            };
        }).ToList();

        return ResultOf<List<ProductComponentModel>>.Success(result);
    }
}
