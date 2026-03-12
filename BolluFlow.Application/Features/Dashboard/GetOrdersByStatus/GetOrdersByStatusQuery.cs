using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Dashboard.GetOrdersByStatus;

public sealed record GetOrdersByStatusQuery : IQuery<ResultOf<List<GetOrdersByStatusResult>>>;
