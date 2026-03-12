using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Orders.OrderItems.UpdateOrderItem;

public record UpdateOrderItemCommand(long OrderId, long OrderItemId, UpdateOrderItemCommandBody Body)
    : ICommand<ResultOf<OrderItemModel>>;

public record UpdateOrderItemCommandBody(int Quantity);
