using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using MediatR;

namespace BolluFlow.Application.Features.StockMovements.ListStockMovements;

public record ListStockMovementsQuery(
    string? ItemType,
    long? ItemId,
    string? MovementType,
    DateTime? StartDate,
    DateTime? EndDate) : IQuery<ResultOf<List<StockMovementModel>>>;
