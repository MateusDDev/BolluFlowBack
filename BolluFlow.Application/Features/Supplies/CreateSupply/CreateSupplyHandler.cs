using BolluFlow.Application.Abstractions;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Supplies.CreateSupply;

public class CreateSupplyHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<CreateSupplyCommand, ResultOf<CreateSupplyResult>>
{
    public async Task<ResultOf<CreateSupplyResult>> Handle(CreateSupplyCommand request, CancellationToken cancellationToken)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var supply = Supply.Create(user.Id, request.Name, request.Unit, request.UnitCost, request.StockQuantity, request.MinimumStockQuantity);
            db.Supplies.Add(supply);
            await db.SaveChangesAsync(cancellationToken);

            if (request.StockQuantity > 0)
            {
                var eventKey = $"initial_stock:supply:{supply.Id}";
                var existingMovement = await db.StockMovements.FirstOrDefaultAsync(x => x.EventKey == eventKey, cancellationToken);

                if (existingMovement is null)
                {
                    var movement = StockMovement.Create(
                        StockItemType.Supply,
                        supply.Id,
                        StockMovementType.Purchase,
                        request.StockQuantity,
                        request.UnitCost,
                        "Estoque inicial",
                        eventKey);

                    db.StockMovements.Add(movement);

                    var existingEntry = await db.FinancialEntries.FirstOrDefaultAsync(x => x.UserId == user.Id && x.EventKey == eventKey, cancellationToken);

                    if (existingEntry is null)
                    {
                        db.FinancialEntries.Add(FinancialEntry.Create(
                            user.Id,
                            FinancialEntryType.Expense,
                            movement.TotalValue,
                            $"Estoque inicial - {supply.Name}",
                            FinancialOriginType.SupplyPurchase,
                            supply.Id,
                            eventKey));
                    }
                }
            }

            await db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return ResultOf<CreateSupplyResult>.Success(new CreateSupplyResult(supply.Id));
        }
        catch (DomainException e)
        {
            await tx.RollbackAsync(cancellationToken);
            return ResultOf<CreateSupplyResult>.BadRequest(e.Error);
        }
    }
}
