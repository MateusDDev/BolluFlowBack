using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Orders.OrderItems.CreateOrderItems;

public record CreateOrderItemsCommand(long OrderId, List<CreateOrderItemCommandBody> Items)
    : ICommand<ResultOf<List<OrderItemModel>>>;

public record CreateOrderItemCommandBody(long ProductId, int Quantity);
