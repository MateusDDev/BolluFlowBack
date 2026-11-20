using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Application.Services;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Errors;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.OrderItems.CreateOrderItems;

public class CreateOrderItemsHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<CreateOrderItemsCommand, ResultOf<List<OrderItemModel>>>
{
    public async Task<ResultOf<List<OrderItemModel>>> Handle(CreateOrderItemsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await db.Orders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == request.OrderId && x.UserId == user.Id, cancellationToken);

            if (order is null)
                return ResultOf<List<OrderItemModel>>.NotFound(CommonErrors.OrderNotFound);

            var orderProductIds = order.Items.Select(x => x.ProductId).ToHashSet();
            if (request.Items.Any(x => orderProductIds.Contains(x.ProductId)))
                return ResultOf<List<OrderItemModel>>.Conflict(new DomainError("ITEM_ALREADY_ADDED", "Item já adicionado ao pedido"));

            var pricing = await OrderPricingService.LoadAsync(db, user.Id, request.Items.Select(x => x.ProductId), cancellationToken);

            foreach (var item in request.Items)
            {
                if (!pricing.TryGetValue(item.ProductId, out var productPricing))
                    return ResultOf<List<OrderItemModel>>.NotFound(CommonErrors.ProductNotFound);

                if (!productPricing.Product.IsActive)
                    return ResultOf<List<OrderItemModel>>.Unprocessable(ProductErrors.ProductInactive);

                order.AddItem(item.ProductId, item.Quantity, productPricing.Product.FinalPrice, productPricing.UnitCost);
            }

            await db.SaveChangesAsync(cancellationToken);

            var created = await db.OrderItems.Where(x => x.OrderId == request.OrderId).Select(x => new OrderItemModel
            {
                Id = x.Id,
                OrderId = x.OrderId,
                ProductId = x.ProductId,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice,
                TotalPrice = x.UnitPrice * x.Quantity,
                Product = new ProductRefModel { Id = x.ProductId, Name = x.Product.Name }
            }).ToListAsync(cancellationToken);

            return ResultOf<List<OrderItemModel>>.Success(created);
        }
        catch (DomainException e)
        {
            return ResultOf<List<OrderItemModel>>.BadRequest(e.Error);
        }
    }
}
