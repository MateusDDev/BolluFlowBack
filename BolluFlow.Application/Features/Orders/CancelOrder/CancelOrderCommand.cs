using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Orders.CancelOrder;

public record CancelOrderCommand(long OrderId, CancelOrderCommandBody Body) : ICommand<Result>;

public record CancelOrderCommandBody(bool? RevertStock);