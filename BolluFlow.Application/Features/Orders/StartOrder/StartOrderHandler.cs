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

namespace BolluFlow.Application.Features.Orders.StartOrder;

public class StartOrderHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<StartOrderCommand, Result>
{
    public async Task<Result> Handle(StartOrderCommand request, CancellationToken cancellationToken)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var order = await db.Orders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == request.OrderId && x.UserId == user.Id, cancellationToken);

            if (order is null)
                return Result.NotFound(CommonErrors.OrderNotFound);

            if (order.Status != OrderStatus.Pending)
                return Result.Unprocessable(OrderErrors.InvalidStartTransition);

            if (order.Items.Count == 0)
                return Result.Unprocessable(new DomainError("ORDER_WITHOUT_ITEMS", "Pedido sem itens não pode ser iniciado"));

            var allowNegativeStock = await OrderFulfillmentService.AllowNegativeStockAsync(db, user.Id, cancellationToken);
            var products = await OrderFulfillmentService.LoadProductsAsync(db, order, cancellationToken);
            var insufficientProducts = OrderFulfillmentService.FindInsufficientStock(order, products, allowNegativeStock);

            if (insufficientProducts.Count > 0)
                return Result.Unprocessable(OrderFulfillmentService.InsufficientStockError, new { insufficientProducts });

            order.Status = OrderStatus.InProduction;
            OrderFulfillmentService.ConsumeStock(order, products, allowNegativeStock);

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
