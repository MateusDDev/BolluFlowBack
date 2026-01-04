using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Orders.ReopenOrder;

public record ReopenOrderCommand(long OrderId) : ICommand<Result>;