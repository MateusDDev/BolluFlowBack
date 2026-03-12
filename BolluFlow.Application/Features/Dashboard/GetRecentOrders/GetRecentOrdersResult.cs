using System.Text.Json.Serialization;

namespace BolluFlow.Application.Features.Dashboard.GetRecentOrders;

public record GetRecentOrdersResult(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("number")] int Number,
    [property: JsonPropertyName("customerName")] string CustomerName,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("createdAt")] DateTime CreatedAt,
    [property: JsonPropertyName("totalItems")] int TotalItems,
    [property: JsonPropertyName("totalValue")] decimal TotalValue);
