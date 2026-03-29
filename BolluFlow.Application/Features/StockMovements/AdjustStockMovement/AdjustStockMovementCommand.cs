using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.StockMovements.AdjustStockMovement;

public class AdjustStockMovementCommand : ICommand<ResultOf<StockAdjustmentModel>>
{
    public string ItemType { get; set; } = null!;
    public long ItemId { get; set; }
    public decimal Quantity { get; set; }
    public string Direction { get; set; } = null!;
    public decimal UnitCost { get; set; }
    public string Reason { get; set; } = null!;
}
