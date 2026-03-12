using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Dashboard.GetInventoryValueByType;

public record GetInventoryValueByTypeQuery : IQuery<ResultOf<List<GetInventoryValueByTypeResult>>>;
