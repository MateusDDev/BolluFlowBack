using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Dashboard.GetRecentOrders;

public record GetRecentOrdersQuery(int? Limit) : IQuery<ResultOf<List<GetRecentOrdersResult>>>;
