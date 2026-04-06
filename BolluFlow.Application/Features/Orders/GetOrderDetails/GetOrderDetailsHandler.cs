using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.GetOrderDetails;

public class GetOrderDetailsHandler(ApplicationDbContext db)
    : IRequestHandler<GetOrderDetailsQuery, ResultOf<OrderModel>>
{
    public async Task<ResultOf<OrderModel>> Handle(GetOrderDetailsQuery request, CancellationToken cancellationToken)
    {
        var order = await db.Orders
            .Include(x => x.Customer)
            .ThenInclude(x => x.DeliveryAddress)
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);

        if (order is null)
            return ResultOf<OrderModel>.BadRequest(CommonErrors.OrderNotFound);

        var subtotal = order.CalculateSubtotal();
        var discountValue = order.CalculateDiscount(subtotal);

        return ResultOf<OrderModel>.Success(new OrderModel
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
        });
    }
}
