using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Ingredients.UnitConversion.ListConversions;

public record ListConversionsQuery(long IngredientId) : IQuery<ResultOf<List<IngredientConversionModel>>>;