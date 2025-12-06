using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Customers.Delete;

public record DeleteCustomerCommand(long CustomerId) : ICommand<Result>;