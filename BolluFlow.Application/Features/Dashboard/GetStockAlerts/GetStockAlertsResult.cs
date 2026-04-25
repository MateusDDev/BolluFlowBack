namespace BolluFlow.Application.Features.Dashboard.GetStockAlerts;

public sealed record GetStockAlertsResult(
    long Id,
    string Name,
    string Category,
    decimal StockQuantity,
    decimal MinimumStockQuantity,
    decimal StockCoveragePercent,
    string Severity,
    string BaseUnit);
