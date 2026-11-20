using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Services;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.CancelOrder;

public class CancelOrderHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<CancelOrderCommand, Result>
{
    public async Task<Result> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var order = await db.Orders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == request.OrderId && x.UserId == user.Id, cancellationToken);

            if (order is null)
                return Result.NotFound(CommonErrors.OrderNotFound);

            var shouldRevertStock = request.RevertStock && order.CanRevertStockOnCancel;

            order.Cancel();

            if (shouldRevertStock)
            {
                var products = await OrderFulfillmentService.LoadProductsAsync(db, order, cancellationToken);
                OrderFulfillmentService.RevertStock(order, products);
            }

            await db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch (DomainException e)
        {
            await tx.RollbackAsync(cancellationToken);
            return Result.BadRequest(e.Error);
        }
    }
}
