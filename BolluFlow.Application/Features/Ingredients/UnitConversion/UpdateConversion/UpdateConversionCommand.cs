using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Ingredients.UnitConversion.UpdateConversion;

public record UpdateConversionCommand(long IngredientId, long ConversionId, UpdateUnitCommandBody Body) : ICommand<Result>;

public record UpdateUnitCommandBody(string Name, decimal QuantityInBaseUnit);