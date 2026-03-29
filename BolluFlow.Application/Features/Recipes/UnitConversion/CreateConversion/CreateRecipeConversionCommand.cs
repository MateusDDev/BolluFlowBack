using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Recipes.UnitConversion.CreateConversion;

public record CreateRecipeConversionCommand(long RecipeId, CreateRecipeUnitCommandBody Body) : ICommand<Result>;

public record CreateRecipeUnitCommandBody(string Name, decimal QuantityInBaseUnit);
