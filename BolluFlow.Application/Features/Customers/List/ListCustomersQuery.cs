using BolluFlow.Application.Abstractions;
using BolluFlow.Domain.Entities;

namespace BolluFlow.Application.Features.Customers.List;

public record ListCustomersQuery : IQuery<ResultOf<List<Customer>>>;