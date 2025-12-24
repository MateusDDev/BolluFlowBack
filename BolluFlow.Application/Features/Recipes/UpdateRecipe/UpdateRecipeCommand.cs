using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Recipes.UpdateRecipe;

public record UpdateRecipeCommand(long RecipeId, UpdateRecipeCommandBody Body) : ICommand<ResultOf<RecipeModel>>;

public record UpdateRecipeCommandBody(
    string Name,
    string Category,
    decimal IndirectCostPercentage,
    decimal MarkupPercentage,
    decimal FinalPrice,
    decimal? DiscountPercentage,
    int? PreparationTime,
    bool IsActive);