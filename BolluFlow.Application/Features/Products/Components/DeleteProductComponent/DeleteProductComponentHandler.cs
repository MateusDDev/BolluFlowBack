using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Products.Components.DeleteProductComponent;

public class DeleteProductComponentHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<DeleteProductComponentCommand, Result>
{
    public async Task<Result> Handle(DeleteProductComponentCommand request, CancellationToken cancellationToken)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        var component = await db.ProductComponents.Include(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == request.ComponentId && x.ProductId == request.ProductId && x.Product.UserId == user.Id,
                cancellationToken);

        if (component is null)
            return Result.NotFound(CommonErrors.ProductComponentNotFound);

        db.ProductComponents.Remove(component);
        await db.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return Result.NoContent();
    }
}
