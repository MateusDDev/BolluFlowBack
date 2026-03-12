using BolluFlow.Application.Abstractions;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Dashboard.GetInventoryValueByType;

public class GetInventoryValueByTypeHandler(ApplicationDbContext db, CurrentUser currentUser)
    : IRequestHandler<GetInventoryValueByTypeQuery, ResultOf<List<GetInventoryValueByTypeResult>>>
{
    public async Task<ResultOf<List<GetInventoryValueByTypeResult>>> Handle(GetInventoryValueByTypeQuery request, CancellationToken cancellationToken)
    {
        // Regra oficial: valuation de produtos no dashboard usa Product.FinalPrice.
        var productsTotal = await db.Products
            .AsNoTracking()
            .Where(x => x.UserId == currentUser.Id)
            .Select(x => x.StockQuantity * x.FinalPrice)
            .DefaultIfEmpty(0m)
            .SumAsync(cancellationToken);

        var ingredientsTotal = await db.Ingredients
            .AsNoTracking()
            .Where(x => x.UserId == currentUser.Id)
            .Select(x => x.StockQuantity * x.UnitCost)
            .DefaultIfEmpty(0m)
            .SumAsync(cancellationToken);

        var suppliesTotal = await db.Supplies
            .AsNoTracking()
            .Where(x => x.UserId == currentUser.Id)
            .Select(x => x.StockQuantity * x.UnitCost)
            .DefaultIfEmpty(0m)
            .SumAsync(cancellationToken);

        var result = new List<GetInventoryValueByTypeResult>
        {
            new("Products", decimal.Round(productsTotal, 2)),
            new("Ingredients", decimal.Round(ingredientsTotal, 2)),
            new("Supplies", decimal.Round(suppliesTotal, 2))
        };

        return ResultOf<List<GetInventoryValueByTypeResult>>.Success(result);
    }
}
