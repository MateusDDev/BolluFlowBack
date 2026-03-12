using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Dashboard.GetOrdersByStatus;

public record GetOrdersByStatusQuery : IQuery<ResultOf<List<GetOrdersByStatusResult>>>;
