using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Orders.CancelOrder;

public record CancelOrderCommand(long OrderId, bool RevertStock) : ICommand<Result>;
