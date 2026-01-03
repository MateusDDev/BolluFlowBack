using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Orders.CreateOrder;

public record CreateOrderCommand(
    long CustomerId,
    string? Note,
    DateTime? DeliveryDate) : ICommand<ResultOf<OrderModel>>;