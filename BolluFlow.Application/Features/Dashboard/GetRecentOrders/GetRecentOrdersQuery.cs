using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Dashboard.GetRecentOrders;

public sealed record GetRecentOrdersQuery(int? Limit = null) : IQuery<ResultOf<List<GetRecentOrdersResult>>>;
