namespace BolluFlow.Application.Models;

public class StockMovementModel
{
    public long Id { get; set; }
    public string ItemType { get; set; } = null!;
    public long ItemId { get; set; }
    public string MovementType { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalValue { get; set; }
    public string? Notes { get; set; }
    public string? EventKey { get; set; }
    public DateTime CreatedAt { get; set; }
}
