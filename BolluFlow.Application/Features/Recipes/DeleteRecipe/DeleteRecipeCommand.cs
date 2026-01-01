using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Recipes.DeleteRecipe;

public record DeleteRecipeCommand(long RecipeId) : ICommand<Result>;