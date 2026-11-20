using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Products.GetProductDetails;

public class GetProductDetailsHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<GetProductDetailsQuery, ResultOf<ProductModel>>
{
    public async Task<ResultOf<ProductModel>> Handle(GetProductDetailsQuery request, CancellationToken cancellationToken)
    {
        var product = await db.Products.FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == user.Id, cancellationToken);
        return product is null
            ? ResultOf<ProductModel>.NotFound(CommonErrors.ProductNotFound)
            : ResultOf<ProductModel>.Success(ProductModel.From(product));
    }
}
