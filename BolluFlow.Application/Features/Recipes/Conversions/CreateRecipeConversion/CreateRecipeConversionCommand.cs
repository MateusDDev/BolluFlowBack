using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Recipes.Conversions.CreateRecipeConversion;

public record CreateRecipeConversionCommand(long RecipeId, RecipeConversionBody Body) : ICommand<Result>;
public record RecipeConversionBody(string Name, decimal QuantityInBaseUnit);
