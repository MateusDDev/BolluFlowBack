using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Recipes.RecipeIngredients.DeleteRecipeIngredient;

public record DeleteRecipeIngredientCommand(long RecipeId, long IngredientId) : ICommand<Result>;