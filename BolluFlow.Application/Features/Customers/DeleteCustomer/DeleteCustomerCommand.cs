using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Customers.DeleteCustomer;

public record DeleteCustomerCommand(long CustomerId) : ICommand<Result>;