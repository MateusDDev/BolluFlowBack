using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Entities;
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
                .FirstOrDefaultAsync(x => x.Id == request.OrderId && x.Customer.UserId == user.Id, cancellationToken);

            if (order is null)
                return Result.NotFound(CommonErrors.OrderNotFound);

            if (order.Status == OrderStatus.Delivered)
                return Result.Success();

            if (order.Status != OrderStatus.Ready)
                return Result.Unprocessable(OrderErrors.InvalidDeliverTransition);

            order.Status = OrderStatus.Delivered;

            var financialEventKey = $"order_delivered:{order.Id}";
            var existingFinancial = await db.FinancialEntries
                .FirstOrDefaultAsync(x => x.UserId == user.Id && x.EventKey == financialEventKey, cancellationToken);

            if (existingFinancial is null)
            {
                var orderTotal = decimal.Round(order.CalculateTotal(), 2, MidpointRounding.AwayFromZero);

                db.FinancialEntries.Add(FinancialEntry.Create(
                    user.Id,
                    FinancialEntryType.Income,
                    orderTotal,
                    $"Venda - Pedido #{order.Number}",
                    FinancialOriginType.Order,
                    order.Id,
                    financialEventKey));
            }

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
