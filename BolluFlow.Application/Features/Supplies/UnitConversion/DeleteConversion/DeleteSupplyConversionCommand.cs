using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Supplies.UnitConversion.DeleteConversion;

public record DeleteSupplyConversionCommand(long SupplyId, long ConversionId) : ICommand<Result>;
