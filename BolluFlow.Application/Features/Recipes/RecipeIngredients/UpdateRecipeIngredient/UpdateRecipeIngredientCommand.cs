using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Recipes.RecipeIngredients.UpdateRecipeIngredient;

public record UpdateRecipeIngredientCommand(long RecipeId, long IngredientId, UpdateRecipeIngredientCommandBody Body)
    : ICommand<Result>;

public record UpdateRecipeIngredientCommandBody(decimal QuantityUsed) : ICommand<Result>;