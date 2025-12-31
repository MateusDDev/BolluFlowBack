using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Recipes.RecipeIngredients.GetRecipeIngredients;

public record GetRecipeIngredientsQuery(long RecipeId) : IQuery<ResultOf<List<GetRecipeIngredientsResult>>>;