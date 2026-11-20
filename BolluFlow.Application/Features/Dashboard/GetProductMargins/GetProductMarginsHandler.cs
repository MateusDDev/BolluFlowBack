using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Services;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Dashboard.GetProductMargins;

public sealed class GetProductMarginsHandler(ApplicationDbContext db, CurrentUser currentUser)
    : IRequestHandler<GetProductMarginsQuery, ResultOf<List<GetProductMarginsResult>>>
{
    public async Task<ResultOf<List<GetProductMarginsResult>>> Handle(GetProductMarginsQuery request, CancellationToken cancellationToken)
    {
        var productIds = await db.Products
            .Where(x => x.UserId == currentUser.Id && x.IsActive)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var productsWithRecipe = await db.ProductComponents
            .Where(x => productIds.Contains(x.ProductId))
            .Select(x => x.ProductId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var pricing = await OrderPricingService.LoadAsync(db, currentUser.Id, productIds, cancellationToken);

        var result = pricing.Values
            .Select(x =>
            {
                var price = x.Product.FinalPrice;
                var hasRecipe = productsWithRecipe.Contains(x.Product.Id);
                decimal? margin = hasRecipe && price > 0
                    ? Math.Round((price - x.UnitCost) / price * 100, 1)
                    : null;

                return new GetProductMarginsResult(
                    x.Product.Id,
                    x.Product.Name,
                    price,
                    decimal.Round(x.UnitCost, 2, MidpointRounding.AwayFromZero),
                    margin,
                    hasRecipe);
            })
            .OrderBy(x => x.HasRecipe)
            .ThenBy(x => x.Margin)
            .ToList();

        return ResultOf<List<GetProductMarginsResult>>.Success(result);
    }
}
