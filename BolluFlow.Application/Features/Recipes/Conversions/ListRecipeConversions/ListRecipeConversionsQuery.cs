using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Recipes.Conversions.ListRecipeConversions;

public record ListRecipeConversionsQuery(long RecipeId) : IQuery<ResultOf<List<RecipeConversionModel>>>;
