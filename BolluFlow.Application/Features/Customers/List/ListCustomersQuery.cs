using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Customers.List;

public record ListCustomersQuery : IQuery<ResultOf<List<CustomerModel>>>;