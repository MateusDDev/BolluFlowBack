using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Orders.FinishOrder;

public record FinishOrderCommand(long OrderId) : ICommand<Result>;