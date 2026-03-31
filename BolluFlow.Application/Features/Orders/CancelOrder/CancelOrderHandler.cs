using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.CancelOrder;

public class CancelOrderHandler(ApplicationDbContext db) : IRequestHandler<CancelOrderCommand, Result>
{
    public async Task<Result> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var order = await db.Orders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);

            if (order is null)
                return Result.NotFound(CommonErrors.OrderNotFound);

            if (order.Status is OrderStatus.Canceled or OrderStatus.Delivered)
            {
                return Result.BadRequest(new DomainError("ORDER_NOT_POSSIBLE_TO_CANCEL",
                    "Não é possível cancelar pedidos já entregues ou já cancelados"));
            }

            var shouldRevertStock = request.RevertStock && order.Status is OrderStatus.InProduction or OrderStatus.Ready;

            if (shouldRevertStock && order.Items.Count > 0)
            {
                var productIds = order.Items.Select(x => x.ProductId).Distinct().ToList();
                var products = await db.Products
                    .Where(x => productIds.Contains(x.Id))
                    .ToDictionaryAsync(x => x.Id, cancellationToken);

                foreach (var item in order.Items)
                {
                    if (!products.TryGetValue(item.ProductId, out var product))
                        return Result.NotFound(CommonErrors.ProductNotFound);

                    product.AddStock(item.Quantity);
                }
            }

            order.Status = OrderStatus.Canceled;

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
