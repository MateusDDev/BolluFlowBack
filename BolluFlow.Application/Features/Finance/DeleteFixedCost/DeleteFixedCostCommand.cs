using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Finance.DeleteFixedCost;

public record DeleteFixedCostCommand(Guid FixedCostId, string Mode, string Month) : ICommand<ResultOf<object>>;
