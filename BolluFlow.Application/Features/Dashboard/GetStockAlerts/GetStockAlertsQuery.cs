using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Dashboard.GetStockAlerts;

public record GetStockAlertsQuery(int? Limit) : IQuery<ResultOf<List<GetStockAlertsResult>>>;
