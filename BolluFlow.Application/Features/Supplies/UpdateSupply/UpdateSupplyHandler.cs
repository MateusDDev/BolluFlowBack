using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
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

            supply.Update(request.Body.Name, request.Body.Unit, request.Body.UnitCost, supply.StockQuantity, request.Body.MinimumStockQuantity);

            if (request.Body.TrackStock.HasValue)
                supply.SetTrackStock(request.Body.TrackStock.Value);

            await db.SaveChangesAsync(cancellationToken);
            return ResultOf<SupplyModel>.Success(SupplyModel.From(supply));
        }
        catch (DomainException e)
        {
            return ResultOf<SupplyModel>.BadRequest(e.Error);
        }
    }
}
