using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Orders.OrderItems.CreateOrderItem;

public record CreateOrderItemCommand(long OrderId, List<CreateOrderItemCommandBody> Items)
    : ICommand<ResultOf<List<OrderItemModel>>>;

public record CreateOrderItemCommandBody(long RecipeId, int Quantity);
