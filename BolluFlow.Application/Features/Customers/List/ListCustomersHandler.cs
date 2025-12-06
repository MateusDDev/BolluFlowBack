using BolluFlow.Application.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Customers.List;

public class ListCustomersHandler(ApplicationDbContext db) : IRequestHandler<ListCustomersQuery, ResultOf<List<Customer>>>
{
    public async Task<ResultOf<List<Customer>>> Handle(ListCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await db.Customers
            .Include(x => x.DeliveryAddress)
            .ToListAsync(cancellationToken);

        return ResultOf<List<Customer>>.Success(customers);
    }
}