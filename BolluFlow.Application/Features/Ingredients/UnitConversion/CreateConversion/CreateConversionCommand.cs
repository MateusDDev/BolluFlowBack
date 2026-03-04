using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Ingredients.UnitConversion.CreateConversion;

public record CreateConversionCommand(long IngredientId, CreateUnitCommandBody Body) : ICommand<Result>;

public record CreateUnitCommandBody(string Name, decimal QuantityInBaseUnit);