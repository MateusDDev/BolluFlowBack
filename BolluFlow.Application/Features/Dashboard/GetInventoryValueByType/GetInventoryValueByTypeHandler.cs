using BolluFlow.Application.Abstractions;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Dashboard.GetInventoryValueByType;

public sealed class GetInventoryValueByTypeHandler(ApplicationDbContext db, CurrentUser currentUser)
    : IRequestHandler<GetInventoryValueByTypeQuery, ResultOf<List<GetInventoryValueByTypeResult>>>
{
    public async Task<ResultOf<List<GetInventoryValueByTypeResult>>> Handle(GetInventoryValueByTypeQuery request, CancellationToken cancellationToken)
    {
        // Regra oficial do dashboard: Products usam FinalPrice como fonte de verdade.
        var products = await db.Products
            .AsNoTracking()
            .Where(x => x.UserId == currentUser.Id)
            .SumAsync(x => x.StockQuantity * x.FinalPrice, cancellationToken);

        var ingredients = await db.Ingredients
            .AsNoTracking()
            .Where(x => x.UserId == currentUser.Id)
            .SumAsync(x => x.StockQuantity * x.UnitCost, cancellationToken);

        var supplies = await db.Supplies
            .AsNoTracking()
            .Where(x => x.UserId == currentUser.Id)
            .SumAsync(x => x.StockQuantity * x.UnitCost, cancellationToken);

        return ResultOf<List<GetInventoryValueByTypeResult>>.Success(
        [
            new GetInventoryValueByTypeResult("Products", decimal.Round(products, 2)),
            new GetInventoryValueByTypeResult("Ingredients", decimal.Round(ingredients, 2)),
            new GetInventoryValueByTypeResult("Supplies", decimal.Round(supplies, 2))
        ]);
    }
}
