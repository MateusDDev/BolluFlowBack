using System.Text.Json.Serialization;

namespace BolluFlow.Application.Features.Dashboard.GetOrdersByStatus;

public record GetOrdersByStatusResult(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("total")] int Total);
