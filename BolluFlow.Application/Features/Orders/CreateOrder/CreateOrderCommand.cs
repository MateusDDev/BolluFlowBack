using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Features.Orders.CreateOrder;

public record CreateOrderCommand(
    long CustomerId,
    string? Note,
    DateTime? DeliveryDate,
    PaymentMethod? PaymentMethod) : ICommand<ResultOf<OrderModel>>;
