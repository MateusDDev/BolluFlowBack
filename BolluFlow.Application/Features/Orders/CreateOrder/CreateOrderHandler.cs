using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Application.Services;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Errors;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.CreateOrder;

public class CreateOrderHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<CreateOrderCommand, ResultOf<OrderModel>>
{
    public async Task<ResultOf<OrderModel>> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        var items = (request.Items ?? [])
            .GroupBy(x => x.ProductId)
            .Select(x => (ProductId: x.Key, Quantity: x.Sum(i => i.Quantity)))
            .ToList();

        if (items.Any(x => x.Quantity <= 0))
            return ResultOf<OrderModel>.BadRequest(new DomainError("ORDER_ITEM_QUANTITY_INVALID", "A quantidade de cada produto precisa ser maior que zero"));

        if (request.DeliverNow && items.Count == 0)
            return ResultOf<OrderModel>.BadRequest(OrderErrors.QuickSaleWithoutItems);

        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            if (request.CustomerId.HasValue)
            {
                var customerExists = await db.Customers
                    .AnyAsync(x => x.Id == request.CustomerId && x.UserId == user.Id, cancellationToken);

                if (!customerExists)
                    return ResultOf<OrderModel>.NotFound(CommonErrors.CustomerNotFound);
            }

            var pricing = await OrderPricingService.LoadAsync(db, user.Id, items.Select(x => x.ProductId), cancellationToken);

            foreach (var item in items)
            {
                if (!pricing.TryGetValue(item.ProductId, out var productPricing))
                    return ResultOf<OrderModel>.NotFound(CommonErrors.ProductNotFound);

                if (!productPricing.Product.IsActive)
                    return ResultOf<OrderModel>.Unprocessable(ProductErrors.ProductInactive);
            }

            var lastNumber = await db.Orders
                .Where(x => x.UserId == user.Id)
                .MaxAsync(x => (int?)x.Number, cancellationToken) ?? 0;

            var order = Order.Create(
                user.Id,
                request.CustomerId,
                lastNumber + 1,
                string.IsNullOrWhiteSpace(request.Note) ? null : request.Note.Trim(),
                request.DeliveryDate,
                request.PaymentMethod);

            foreach (var item in items)
            {
                var productPricing = pricing[item.ProductId];
                order.AddItem(item.ProductId, item.Quantity, productPricing.Product.FinalPrice, productPricing.UnitCost);
            }

            if (request.DeliverNow)
            {
                var allowNegativeStock = await OrderFulfillmentService.AllowNegativeStockAsync(db, user.Id, cancellationToken);
                var products = await OrderFulfillmentService.LoadProductsAsync(db, order, cancellationToken);
                var insufficientProducts = OrderFulfillmentService.FindInsufficientStock(order, products, allowNegativeStock);

                if (insufficientProducts.Count > 0)
                    return ResultOf<OrderModel>.Unprocessable(OrderFulfillmentService.InsufficientStockError, new { insufficientProducts });

                OrderFulfillmentService.ConsumeStock(order, products, allowNegativeStock);
                order.CompleteAsQuickSale();
            }

            db.Orders.Add(order);
            await db.SaveChangesAsync(cancellationToken);

            if (request.DeliverNow)
            {
                await OrderFulfillmentService.RegisterIncomeAsync(db, order, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
            }

            await tx.CommitAsync(cancellationToken);

            if (order.CustomerId.HasValue)
            {
                await db.Entry(order).Reference(x => x.Customer).LoadAsync(cancellationToken);
                await db.Entry(order.Customer!).Reference(x => x.DeliveryAddress).LoadAsync(cancellationToken);
            }

            return ResultOf<OrderModel>.Created(OrderModel.From(order));
        }
        catch (DomainException e)
        {
            await tx.RollbackAsync(cancellationToken);
            return ResultOf<OrderModel>.BadRequest(e.Error);
        }
    }
}
