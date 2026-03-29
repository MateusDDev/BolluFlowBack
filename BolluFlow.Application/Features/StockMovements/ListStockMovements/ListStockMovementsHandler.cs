using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.StockMovements.ListStockMovements;

public class ListStockMovementsHandler(ApplicationDbContext db)
    : IRequestHandler<ListStockMovementsQuery, ResultOf<List<StockMovementModel>>>
{
    public async Task<ResultOf<List<StockMovementModel>>> Handle(ListStockMovementsQuery request, CancellationToken cancellationToken)
    {
        var query = db.StockMovements.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.ItemType))
        {
            if (!Enum.TryParse<StockItemType>(request.ItemType, true, out var itemType))
                return ResultOf<List<StockMovementModel>>.BadRequest(new("STOCK_ITEM_TYPE_INVALID", "item_type inválido"));

            query = query.Where(x => x.ItemType == itemType);
        }

        if (request.ItemId.HasValue)
            query = query.Where(x => x.ItemId == request.ItemId.Value);

        if (!string.IsNullOrWhiteSpace(request.MovementType))
        {
            if (!Enum.TryParse<StockMovementType>(request.MovementType, true, out var movementType))
                return ResultOf<List<StockMovementModel>>.BadRequest(new("STOCK_MOVEMENT_TYPE_INVALID", "movement_type inválido"));

            query = query.Where(x => x.MovementType == movementType);
        }

        if (request.StartDate.HasValue)
            query = query.Where(x => x.CreatedAt >= request.StartDate.Value);

        if (request.EndDate.HasValue)
            query = query.Where(x => x.CreatedAt <= request.EndDate.Value);

        var movements = await query
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new StockMovementModel
            {
                Id = x.Id,
                ItemType = x.ItemType.ToString().ToLower(),
                ItemId = x.ItemId,
                MovementType = x.MovementType.ToString().ToLower(),
                Quantity = x.Quantity,
                UnitCost = x.UnitCost,
                TotalValue = x.TotalValue,
                Notes = x.Notes,
                EventKey = x.EventKey,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return ResultOf<List<StockMovementModel>>.Success(movements);
    }
}
