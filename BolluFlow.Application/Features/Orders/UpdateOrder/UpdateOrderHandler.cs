using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.UpdateOrder;

public class UpdateOrderHandler(ApplicationDbContext db) : IRequestHandler<UpdateOrderCommand, ResultOf<OrderModel>>
{
    public async Task<ResultOf<OrderModel>> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var command = request.Body;

            var order = await db.Orders
                .Include(x => x.Customer)
                .ThenInclude(x => x!.DeliveryAddress)
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);

            if (order is null)
                return ResultOf<OrderModel>.NotFound(CommonErrors.OrderNotFound);

            var originalCustomerId = order.CustomerId;

            order.Update(
                command.Note,
                command.DeliveryDate,
                command.CustomerId,
                command.PaymentMethod,
                command.Discount,
                command.DiscountType,
                command.ShippingFee ?? 0);

            await db.SaveChangesAsync(cancellationToken);

            if (command.CustomerId.HasValue && command.CustomerId != originalCustomerId)
            {
                await db.Entry(order).Reference(x => x.Customer).LoadAsync(cancellationToken);
                await db.Entry(order.Customer!).Reference(x => x.DeliveryAddress).LoadAsync(cancellationToken);
            }

            return ResultOf<OrderModel>.Success(OrderModel.From(order));
        }
        catch (DomainException e)
        {
            return ResultOf<OrderModel>.BadRequest(e.Error);
        }
    }
}
