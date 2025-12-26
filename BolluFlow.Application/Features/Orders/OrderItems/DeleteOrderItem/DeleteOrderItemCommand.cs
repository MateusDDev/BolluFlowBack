using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Orders.OrderItems.DeleteOrderItem;

public record DeleteOrderItemCommand(long OrderId, long ItemId) : ICommand<Result>;