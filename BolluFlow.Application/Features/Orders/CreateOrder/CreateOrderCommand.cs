using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Orders.CreateOrder;

public record CreateOrderCommand(
    long CustomerId,
    string? Note,
    DateTime? DeliveryDate,
    List<CreateOrderCommandItemModel> Items) : ICommand<ResultOf<OrderModel>>;

public record CreateOrderCommandItemModel(
    long RecipeId,
    int Quantity);