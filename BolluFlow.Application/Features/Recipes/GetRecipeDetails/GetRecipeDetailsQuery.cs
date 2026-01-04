using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Recipes.GetRecipeDetails;

public record GetRecipeDetailsQuery(long RecipeId) : IQuery<ResultOf<RecipeModel>>;