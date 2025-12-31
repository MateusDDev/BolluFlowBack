using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Recipes.RecipeIngredients.CreateRecipeIngredient;

public record CreateRecipeIngredientCommand(long RecipeId, CreateRecipeIngredientCommandBody Body) : ICommand<Result>;

public record CreateRecipeIngredientCommandBody(long IngredientId, decimal QuantityUsed) : ICommand<Result>;