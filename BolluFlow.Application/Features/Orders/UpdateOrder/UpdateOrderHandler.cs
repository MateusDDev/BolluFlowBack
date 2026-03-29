using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.UpdateOrder;

public class UpdateOrderHandler(ApplicationDbContext db) : IRequestHandler<UpdateOrderCommand, ResultOf<OrderModel>>
{
    public async Task<ResultOf<OrderModel>> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var command = request.Body;

        var order = await db.Orders
            .FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);
        
        if (order is null)
            return ResultOf<OrderModel>.NotFound(CommonErrors.OrderNotFound);

        order.Update(command.Note, command.DeliveryDate, command.CustomerId, command.PaymentMethod);
        
        await db.SaveChangesAsync(cancellationToken);

        return ResultOf<OrderModel>.Success(order.Adapt<OrderModel>());
    }
}
