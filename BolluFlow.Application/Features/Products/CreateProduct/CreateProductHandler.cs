using BolluFlow.Application.Abstractions;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;

namespace BolluFlow.Application.Features.Products.CreateProduct;

public class CreateProductHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<CreateProductCommand, ResultOf<CreateProductResult>>
{
    public async Task<ResultOf<CreateProductResult>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = Product.Create(user.Id, request.Name, request.SalePrice, request.StockQuantity, request.MinimumStockQuantity);
            db.Products.Add(product);
            await db.SaveChangesAsync(cancellationToken);
            return ResultOf<CreateProductResult>.Success(new CreateProductResult(product.Id));
        }
        catch (DomainException e)
        {
            return ResultOf<CreateProductResult>.BadRequest(e.Error);
        }
    }
}
