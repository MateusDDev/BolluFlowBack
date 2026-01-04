using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Orders.OrderItems.ListOrderItems;

public record ListOrderItemsQuery(long OrderId) : IQuery<ResultOf<List<ListOrderItemsResult>>>;