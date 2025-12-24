using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Recipes.ListRecipes;

public record ListRecipesQuery : IQuery<ResultOf<List<RecipeModel>>>;