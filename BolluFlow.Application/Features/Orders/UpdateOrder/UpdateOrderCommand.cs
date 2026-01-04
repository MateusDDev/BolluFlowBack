using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Orders.UpdateOrder;

public record UpdateOrderCommand(long OrderId, UpdateOrderCommandBody Body) : ICommand<ResultOf<OrderModel>>;

public record UpdateOrderCommandBody(string? Note, DateTime? DeliveryDate, long? CustomerId);