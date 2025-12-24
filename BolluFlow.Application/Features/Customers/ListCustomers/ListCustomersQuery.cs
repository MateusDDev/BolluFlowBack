using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Customers.ListCustomers;

public record ListCustomersQuery(long UserId) : IQuery<ResultOf<List<CustomerModel>>>;