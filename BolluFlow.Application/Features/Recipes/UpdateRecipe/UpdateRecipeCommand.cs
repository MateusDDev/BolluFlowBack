using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Features.Recipes.UpdateRecipe;

public record UpdateRecipeCommand(long RecipeId, UpdateRecipeCommandBody Body) : ICommand<ResultOf<RecipeModel>>;

public record UpdateRecipeCommandBody(
    string Name,
    string Category,
    BaseUnit BaseUnit,
    decimal? Yield,
    int? PreparationTime,
    bool IsActive,
    decimal? FinalPrice,
    bool? TrackStock = null,
    OptionalField<decimal?> MinimumStockQuantity = default);
