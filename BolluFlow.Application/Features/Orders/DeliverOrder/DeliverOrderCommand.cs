using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Orders.DeliverOrder;

public record DeliverOrderCommand(long OrderId) : ICommand<Result>;