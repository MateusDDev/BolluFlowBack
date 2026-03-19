using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Financial;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;

namespace BolluFlow.Application.Features.Supplies.CreateSupply;

public class CreateSupplyHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<CreateSupplyCommand, ResultOf<CreateSupplyResult>>
{
    public async Task<ResultOf<CreateSupplyResult>> Handle(CreateSupplyCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var supply = Supply.Create(user.Id, request.Name, request.Unit, request.UnitCost, request.StockQuantity, request.MinimumStockQuantity);
            db.Supplies.Add(supply);
            await db.SaveChangesAsync(cancellationToken);

            await FinancialStockPurchaseSync.RegisterSupplyPurchaseAsync(
                db,
                user.Id,
                supply,
                request.StockQuantity,
                request.UnitCost,
                cancellationToken);

            return ResultOf<CreateSupplyResult>.Success(new CreateSupplyResult(supply.Id));
        }
        catch (DomainException e)
        {
            return ResultOf<CreateSupplyResult>.BadRequest(e.Error);
        }
    }
}
