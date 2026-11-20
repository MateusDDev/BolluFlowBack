using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Supplies.GetSupplyDetails;

public class GetSupplyDetailsHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<GetSupplyDetailsQuery, ResultOf<SupplyModel>>
{
    public async Task<ResultOf<SupplyModel>> Handle(GetSupplyDetailsQuery request, CancellationToken cancellationToken)
    {
        var supply = await db.Supplies.FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == user.Id, cancellationToken);
        return supply is null
            ? ResultOf<SupplyModel>.NotFound(CommonErrors.SupplyNotFound)
            : ResultOf<SupplyModel>.Success(SupplyModel.From(supply));
    }
}
