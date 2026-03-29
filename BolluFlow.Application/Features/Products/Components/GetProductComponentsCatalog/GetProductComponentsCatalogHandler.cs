using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Products.Components.GetProductComponentsCatalog;

public class GetProductComponentsCatalogHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<GetProductComponentsCatalogQuery, ResultOf<List<ProductComponentCatalogItemModel>>>
{
    public async Task<ResultOf<List<ProductComponentCatalogItemModel>>> Handle(GetProductComponentsCatalogQuery request, CancellationToken cancellationToken)
    {
        var exists = await db.Products.AnyAsync(x => x.Id == request.ProductId && x.UserId == user.Id, cancellationToken);
        if (!exists)
            return ResultOf<List<ProductComponentCatalogItemModel>>.NotFound(CommonErrors.ProductNotFound);

        var recipes = await db.Recipes.Where(x => x.UserId == user.Id)
            .Select(x => new ProductComponentCatalogItemModel { Id = x.Id, Name = x.Name, BaseUnit = x.BaseUnit.ToString(), SourceType = "Recipe" })
            .ToListAsync(cancellationToken);

        var ingredients = await db.Ingredients.Where(x => x.UserId == user.Id)
            .Select(x => new ProductComponentCatalogItemModel { Id = x.Id, Name = x.Name, BaseUnit = x.BaseUnit.ToString(), SourceType = "Ingredient" })
            .ToListAsync(cancellationToken);

        var supplies = await db.Supplies.Where(x => x.UserId == user.Id)
            .Select(x => new ProductComponentCatalogItemModel { Id = x.Id, Name = x.Name, BaseUnit = x.BaseUnit.ToString(), SourceType = "Supply" })
            .ToListAsync(cancellationToken);

        return ResultOf<List<ProductComponentCatalogItemModel>>.Success([.. recipes, .. ingredients, .. supplies]);
    }
}
