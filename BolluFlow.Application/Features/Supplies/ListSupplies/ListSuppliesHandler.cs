using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Supplies.ListSupplies;

public class ListSuppliesHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<ListSuppliesQuery, ResultOf<List<SupplyModel>>>
{
    public async Task<ResultOf<List<SupplyModel>>> Handle(ListSuppliesQuery request, CancellationToken cancellationToken)
    {
        var supplies = await db.Supplies.Where(x => x.UserId == user.Id).ToListAsync(cancellationToken);
        return ResultOf<List<SupplyModel>>.Success(supplies.Select(SupplyModel.From).ToList());
    }
}
