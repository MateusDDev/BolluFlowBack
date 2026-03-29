using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Recipes.UnitConversion.ListConversions;

public record ListRecipeConversionsQuery(long RecipeId) : IQuery<ResultOf<List<RecipeConversionModel>>>;
