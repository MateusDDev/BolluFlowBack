using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Orders.DeleteOrder;

public record DeleteOrderCommand(long OrderId) : ICommand<Result>;
