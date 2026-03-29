using System.Text.Json.Serialization;
using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.StockMovements.CreateStockMovement;

public class CreateStockMovementCommand : ICommand<ResultOf<StockMovementModel>>
{
    [JsonPropertyName("item_type")]
    public string ItemType { get; set; } = null!;

    [JsonPropertyName("item_id")]
    public long ItemId { get; set; }

    [JsonPropertyName("movement_type")]
    public string MovementType { get; set; } = null!;

    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; }

    [JsonPropertyName("unit_cost")]
    public decimal UnitCost { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("event_key")]
    public string? EventKey { get; set; }
}
