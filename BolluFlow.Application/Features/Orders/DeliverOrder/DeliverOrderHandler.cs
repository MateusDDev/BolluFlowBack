using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Services;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Enums;
using BolluFlow.Domain.Errors;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.DeliverOrder;

public class DeliverOrderHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<DeliverOrderCommand, Result>
{
    public async Task<Result> Handle(DeliverOrderCommand request, CancellationToken cancellationToken)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var order = await db.Orders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == request.OrderId && x.UserId == user.Id, cancellationToken);

            if (order is null)
                return Result.NotFound(CommonErrors.OrderNotFound);

            if (order.Status == OrderStatus.Delivered)
                return Result.Success();

            if (order.Status != OrderStatus.Ready)
                return Result.Unprocessable(OrderErrors.InvalidDeliverTransition);

            order.MarkDelivered();

            await OrderFulfillmentService.RegisterIncomeAsync(db, order, cancellationToken);

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
