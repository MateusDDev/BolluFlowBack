using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Finance.Shared;

namespace BolluFlow.Application.Features.Finance.ListFixedCosts;

public record ListFixedCostsQuery() : IQuery<ResultOf<List<FixedCostDto>>>;
