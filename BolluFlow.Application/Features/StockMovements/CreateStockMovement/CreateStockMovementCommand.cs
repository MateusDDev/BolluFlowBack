using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.StockMovements.CreateStockMovement;

public class CreateStockMovementCommand : ICommand<ResultOf<StockMovementModel>>
{
    public string ItemType { get; set; } = null!;
    public long ItemId { get; set; }
    public string MovementType { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public string? Notes { get; set; }
    public string? EventKey { get; set; }
}
