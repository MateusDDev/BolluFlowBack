using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Dashboard.GetStockAlerts;

public sealed record GetStockAlertsQuery(int? Limit = null) : IQuery<ResultOf<List<GetStockAlertsResult>>>;
