using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Finance.Shared;

namespace BolluFlow.Application.Features.Finance.UpdateFixedCost;

public record UpdateFixedCostCommand(
    Guid FixedCostId,
    string Name,
    decimal Amount,
    string StartMonth,
    string? EndMonth,
    string? Notes) : ICommand<ResultOf<FixedCostDto>>;
