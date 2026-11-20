using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Features.StockMovements.CreateStockMovement;

public record CreateStockMovementCommand(
    StockItemType ItemType,
    long ItemId,
    StockMovementType MovementType,
    decimal Quantity,
    decimal UnitCost,
    string? Notes,
    string? EventKey,
    bool UpdateItemCost = false
) : ICommand<ResultOf<StockMovementModel>>;
