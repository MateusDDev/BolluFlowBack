using System.Text.Json.Serialization;

namespace BolluFlow.Application.Features.Dashboard.GetStockAlerts;

public record GetStockAlertsResult(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("category")] string Category,
    [property: JsonPropertyName("stockQuantity")] decimal StockQuantity,
    [property: JsonPropertyName("minimumStockQuantity")] decimal MinimumStockQuantity,
    [property: JsonPropertyName("stockCoveragePercent")] decimal StockCoveragePercent,
    [property: JsonPropertyName("severity")] string Severity);
