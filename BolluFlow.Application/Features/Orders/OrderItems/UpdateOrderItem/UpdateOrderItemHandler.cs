using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.OrderItems.UpdateOrderItem;

public class UpdateOrderItemHandler(ApplicationDbContext db) : IRequestHandler<UpdateOrderItemCommand, ResultOf<OrderItemModel>>
{
    public async Task<ResultOf<OrderItemModel>> Handle(UpdateOrderItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await db.Orders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);

            if (order is null)
                return ResultOf<OrderItemModel>.NotFound(CommonErrors.OrderNotFound);

            var current = order.Items.FirstOrDefault(x => x.Id == request.OrderItemId);
            if (current is null)
                return ResultOf<OrderItemModel>.NotFound(new DomainError("ORDER_ITEM_NOT_FOUND", "Item não encontrado"));

            var item = order.UpdateItem(request.OrderItemId, request.Body.Quantity, current.UnitPrice, current.UnitCost);

            await db.SaveChangesAsync(cancellationToken);

            return ResultOf<OrderItemModel>.Success(new OrderItemModel
            {
                Id = item.Id,
                OrderId = item.OrderId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.UnitPrice * item.Quantity,
                Product = new ProductRefModel { Id = item.ProductId, Name = (await db.Products.FindAsync([item.ProductId], cancellationToken))?.Name ?? string.Empty }
            });
        }
        catch (DomainException e)
        {
            return ResultOf<OrderItemModel>.BadRequest(e.Error);
        }
    }
}
