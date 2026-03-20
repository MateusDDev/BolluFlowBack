using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Finance.Shared;

namespace BolluFlow.Application.Features.Finance.CreateFixedCost;

public record CreateFixedCostCommand(
    string Name,
    decimal Amount,
    string StartMonth,
    string? EndMonth,
    string? Notes) : ICommand<ResultOf<FixedCostDto>>;
