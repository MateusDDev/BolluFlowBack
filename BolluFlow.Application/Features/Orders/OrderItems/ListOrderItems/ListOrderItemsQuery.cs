using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Orders.OrderItems.ListOrderItems;

public record ListOrderItemsQuery(long OrderId) : IQuery<ResultOf<List<OrderItemModel>>>;