using System.Text.Json.Serialization;

namespace BolluFlow.Application.Features.Dashboard.GetInventoryValueByType;

public record GetInventoryValueByTypeResult(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("totalValue")] decimal TotalValue);
