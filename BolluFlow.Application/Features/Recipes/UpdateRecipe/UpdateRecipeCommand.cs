using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Recipes.UpdateRecipe;

public record UpdateRecipeCommand(long RecipeId, UpdateRecipeCommandBody Body) : ICommand<ResultOf<RecipeModel>>;

public record UpdateRecipeCommandBody(
    string Name,
    string Category,
    int? PreparationTime,
    bool IsActive,
    decimal? FinalPrice);