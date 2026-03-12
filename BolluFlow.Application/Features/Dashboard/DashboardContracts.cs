using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BolluFlow.Application.Features.Dashboard;

public sealed class DashboardLimitQuery
{
    [Range(1, 50)]
    public int? Limit { get; init; }
}

public sealed class DashboardRecentOrdersLimitQuery
{
    [Range(1, 20)]
    public int? Limit { get; init; }
}

public sealed record StockAlertResponse(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("category")] string Category,
    [property: JsonPropertyName("stockQuantity")] decimal StockQuantity,
    [property: JsonPropertyName("minimumStockQuantity")] decimal MinimumStockQuantity,
    [property: JsonPropertyName("stockCoveragePercent")] decimal StockCoveragePercent,
    [property: JsonPropertyName("severity")] string Severity);

public sealed record OrdersByStatusResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("total")] int Total);

public sealed record InventoryValueByTypeResponse(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("totalValue")] decimal TotalValue);

public sealed record RecentOrderResponse(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("number")] int Number,
    [property: JsonPropertyName("customerName")] string CustomerName,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("createdAt")] DateTime CreatedAt,
    [property: JsonPropertyName("totalItems")] int TotalItems,
    [property: JsonPropertyName("totalValue")] decimal TotalValue);
