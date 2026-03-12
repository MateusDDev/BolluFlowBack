using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Supplies.UnitConversion.UpdateConversion;

public record UpdateSupplyConversionCommand(long SupplyId, long ConversionId, UpdateSupplyUnitCommandBody Body) : ICommand<Result>;

public record UpdateSupplyUnitCommandBody(string Name, decimal QuantityInBaseUnit);
