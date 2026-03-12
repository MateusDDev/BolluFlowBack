using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Products.UpdateProductPricing;

public class UpdateProductPricingHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<UpdateProductPricingCommand, Result>
{
    public async Task<Result> Handle(UpdateProductPricingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await db.Products.FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == user.Id, cancellationToken);
            if (product is null)
                return Result.NotFound(CommonErrors.ProductNotFound);

            product.UpdatePricing(request.Body.IndirectCostPercentage, request.Body.MarkupPercentage, request.Body.FinalPrice, request.Body.DiscountPercentage);
            await db.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (DomainException e)
        {
            return Result.BadRequest(e.Error);
        }
    }
}
