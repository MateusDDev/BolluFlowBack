using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Orders.StartOrder;

public record StartOrderCommand(long OrderId) : ICommand<Result>;