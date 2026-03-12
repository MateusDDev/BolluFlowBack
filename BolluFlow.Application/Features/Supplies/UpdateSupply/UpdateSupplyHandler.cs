using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Supplies.UpdateSupply;

public class UpdateSupplyHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<UpdateSupplyCommand, ResultOf<SupplyModel>>
{
    public async Task<ResultOf<SupplyModel>> Handle(UpdateSupplyCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var supply = await db.Supplies.FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == user.Id, cancellationToken);
            if (supply is null)
                return ResultOf<SupplyModel>.NotFound(CommonErrors.SupplyNotFound);

            supply.Update(request.Body.Name, request.Body.Unit, request.Body.UnitCost, request.Body.StockQuantity, request.Body.MinimumStockQuantity);
            await db.SaveChangesAsync(cancellationToken);
            return ResultOf<SupplyModel>.Success(supply.Adapt<SupplyModel>());
        }
        catch (DomainException e)
        {
            return ResultOf<SupplyModel>.BadRequest(e.Error);
        }
    }
}
