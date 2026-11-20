using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Products.ListProducts;

public class ListProductsHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<ListProductsQuery, ResultOf<List<ProductModel>>>
{
    public async Task<ResultOf<List<ProductModel>>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await db.Products.Where(x => x.UserId == user.Id).ToListAsync(cancellationToken);
        return ResultOf<List<ProductModel>>.Success(products.Select(ProductModel.From).ToList());
    }
}
