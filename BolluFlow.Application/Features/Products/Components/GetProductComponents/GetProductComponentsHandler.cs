using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Products.Components.GetProductComponents;

public class GetProductComponentsHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<GetProductComponentsQuery, ResultOf<List<ProductComponentModel>>>
{
    public async Task<ResultOf<List<ProductComponentModel>>> Handle(GetProductComponentsQuery request, CancellationToken cancellationToken)
    {
        var items = await db.ProductComponents
            .Where(x => x.ProductId == request.ProductId && x.Product.UserId == user.Id)
            .Select(x => new ProductComponentModel
            {
                Id = x.Id,
                ProductId = x.ProductId,
                ComponentType = x.ComponentType,
                RecipeId = x.RecipeId,
                IngredientId = x.IngredientId,
                SupplyId = x.SupplyId,
                Name = x.Name,
                BaseUnit = x.BaseUnit,
                UnitPrice = x.UnitPrice,
                QuantityUsed = x.QuantityUsed,
                StockQuantity = x.ComponentType == ProductComponentType.Recipe
                    ? x.Recipe!.StockQuantity
                    : x.ComponentType == ProductComponentType.Ingredient
                        ? x.Ingredient!.StockQuantity
                        : x.Supply!.StockQuantity
            })
            .ToListAsync(cancellationToken);

        return ResultOf<List<ProductComponentModel>>.Success(items);
    }
}
