using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Products.Components.UpdateProductComponent;

public class UpdateProductComponentHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<UpdateProductComponentCommand, Result>
{
    public async Task<Result> Handle(UpdateProductComponentCommand request, CancellationToken cancellationToken)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var component = await db.ProductComponents.Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == request.ComponentId && x.ProductId == request.ProductId && x.Product.UserId == user.Id,
                    cancellationToken);

            if (component is null)
                return Result.NotFound(CommonErrors.ProductComponentNotFound);

            component.Update(request.Body.QuantityUsed, component.Name, component.BaseUnit, component.UnitPrice);
            await db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (DomainException e)
        {
            await tx.RollbackAsync(cancellationToken);
            return Result.Unprocessable(e.Error);
        }
    }
}
