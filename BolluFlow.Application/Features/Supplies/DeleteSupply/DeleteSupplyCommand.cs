using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Supplies.DeleteSupply;

public record DeleteSupplyCommand(long Id) : ICommand<Result>;
