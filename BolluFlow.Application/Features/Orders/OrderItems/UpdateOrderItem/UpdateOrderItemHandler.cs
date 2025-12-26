using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.OrderItems.UpdateOrderItem;

public class UpdateOrderItemHandler(ApplicationDbContext db) : IRequestHandler<UpdateOrderItemCommand, ResultOf<OrderItemModel>>
{
    public async Task<ResultOf<OrderItemModel>> Handle(UpdateOrderItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var command = request.Body;

            var order = await db.Orders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);

            if (order is null)
                return ResultOf<OrderItemModel>.BadRequest(CommonErrors.OrderNotFound);

            var item = order.UpdateItem(request.OrderItemId, command.Quantity, command.UnitPrice, command.UnitCost);

            await db.SaveChangesAsync(cancellationToken);

            return ResultOf<OrderItemModel>.Success(item.Adapt<OrderItemModel>());
        }
        catch (DomainException e)
        {
            return ResultOf<OrderItemModel>.BadRequest(e.Error);
        }
    }
}