using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Dashboard.GetInventoryValueByType;

public sealed record GetInventoryValueByTypeQuery : IQuery<ResultOf<List<GetInventoryValueByTypeResult>>>;
