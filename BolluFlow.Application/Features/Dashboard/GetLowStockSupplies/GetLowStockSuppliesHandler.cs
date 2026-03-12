using BolluFlow.Application.Abstractions;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Dashboard.GetLowStockSupplies;

public class GetLowStockSuppliesHandler(ApplicationDbContext db, CurrentUser currentUser)
    : IRequestHandler<GetLowStockSuppliesQuery, ResultOf<List<GetLowStockSuppliesResult>>>
{
    public async Task<ResultOf<List<GetLowStockSuppliesResult>>> Handle(GetLowStockSuppliesQuery request, CancellationToken cancellationToken)
    {
        var supplies = await db.Supplies
            .Where(x => x.MinimumStockQuantity.HasValue && x.StockQuantity < x.MinimumStockQuantity && currentUser.Id == x.UserId)
            .OrderBy(x => x.StockQuantity)
            .Take(10)
            .Select(x => new GetLowStockSuppliesResult(x.Id, x.Name, x.StockQuantity, x.MinimumStockQuantity ?? 0))
            .ToListAsync(cancellationToken);

        return ResultOf<List<GetLowStockSuppliesResult>>.Success(supplies);
    }
}
