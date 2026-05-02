using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Products.UpdateProduct;

public class UpdateProductHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<UpdateProductCommand, ResultOf<ProductModel>>
{
    public async Task<ResultOf<ProductModel>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await db.Products.FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == user.Id, cancellationToken);
            if (product is null)
                return ResultOf<ProductModel>.NotFound(CommonErrors.ProductNotFound);

            product.Update(
                request.Body.Name ?? product.Name,
                request.Body.Category ?? product.Category,
                request.Body.SalePrice ?? product.SalePrice,
                product.StockQuantity,
                request.Body.MinimumStockQuantity ?? product.MinimumStockQuantity,
                request.Body.IsActive);

            await db.SaveChangesAsync(cancellationToken);
            return ResultOf<ProductModel>.Success(product.Adapt<ProductModel>());
        }
        catch (DomainException e)
        {
            return ResultOf<ProductModel>.BadRequest(e.Error);
        }
    }
}
