using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Supplies.UnitConversion.CreateConversion;

public record CreateSupplyConversionCommand(long SupplyId, CreateSupplyUnitCommandBody Body) : ICommand<Result>;

public record CreateSupplyUnitCommandBody(string Name, decimal QuantityInBaseUnit);
