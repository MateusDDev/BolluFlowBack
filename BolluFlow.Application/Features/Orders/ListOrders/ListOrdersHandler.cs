using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.ListOrders;

public class ListOrdersHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<ListOrdersQuery, ResultOf<List<OrderModel>>>
{
    public async Task<ResultOf<List<OrderModel>>> Handle(ListOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await db.Orders
            .Include(x => x.Customer)
            .ThenInclude(x => x.DeliveryAddress)
            .Include(x => x.Items)
            .Where(x => x.Customer.UserId == user.Id)
            .ToListAsync(cancellationToken);

        var response = orders.Select(order =>
        {
            var subtotal = order.CalculateSubtotal();
            var discountValue = order.CalculateDiscount(subtotal);

            return new OrderModel
            {
                Id = order.Id,
                Number = order.Number,
                Customer = order.Customer.Adapt<CustomerModel>(),
                Status = order.Status,
                Note = order.Note,
                CreatedAt = order.CreatedAt,
                DeliveryDate = order.DeliveryDate,
                PaymentMethod = order.PaymentMethod,
                Subtotal = subtotal,
                Discount = order.Discount,
                DiscountType = order.DiscountType,
                DiscountValue = discountValue,
                ShippingFee = order.ShippingFee,
                Total = subtotal - discountValue + order.ShippingFee
            };
        }).ToList();

        return ResultOf<List<OrderModel>>.Success(response);
    }
}
