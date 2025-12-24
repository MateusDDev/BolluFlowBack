using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Orders.ListOrders;

public record ListOrdersQuery : IQuery<ResultOf<List<OrderModel>>>;