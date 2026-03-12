using BolluFlow.Application.Abstractions;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Dashboard.GetLowStockProducts;

public class GetLowStockProductsHandler(ApplicationDbContext db, CurrentUser currentUser)
    : IRequestHandler<GetLowStockProductsQuery, ResultOf<List<GetLowStockProductsResult>>>
{
    public async Task<ResultOf<List<GetLowStockProductsResult>>> Handle(GetLowStockProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await db.Products
            .Where(x => x.MinimumStockQuantity.HasValue && x.StockQuantity < x.MinimumStockQuantity && currentUser.Id == x.UserId)
            .OrderBy(x => x.StockQuantity)
            .Take(10)
            .Select(x => new GetLowStockProductsResult(x.Id, x.Name, x.StockQuantity, x.MinimumStockQuantity ?? 0))
            .ToListAsync(cancellationToken);

        return ResultOf<List<GetLowStockProductsResult>>.Success(products);
    }
}
