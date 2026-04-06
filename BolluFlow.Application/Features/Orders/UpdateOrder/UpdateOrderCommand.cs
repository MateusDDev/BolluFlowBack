using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Features.Orders.UpdateOrder;

public record UpdateOrderCommand(long OrderId, UpdateOrderCommandBody Body) : ICommand<ResultOf<OrderModel>>;

public record UpdateOrderCommandBody(
    string? Note,
    DateTime? DeliveryDate,
    long? CustomerId,
    PaymentMethod? PaymentMethod,
    decimal? Discount,
    DiscountType? DiscountType,
    decimal? ShippingFee);
