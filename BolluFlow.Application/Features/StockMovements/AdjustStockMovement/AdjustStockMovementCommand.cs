using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Features.StockMovements.AdjustStockMovement;

public record AdjustStockMovementCommand(
    StockItemType ItemType,
    long ItemId,
    decimal Quantity,
    AdjustmentDirection Direction,
    decimal UnitCost,
    string Reason
) : ICommand<ResultOf<StockAdjustmentModel>>;
