using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Customers.List;

public class ListCustomersHandler(ApplicationDbContext db) : 
    IRequestHandler<ListCustomersQuery, ResultOf<List<CustomerModel>>>
{
    public async Task<ResultOf<List<CustomerModel>>> Handle(ListCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await db.Customers
            .Include(x => x.DeliveryAddress)
            .ToListAsync(cancellationToken);

        var response = customers.Adapt<List<CustomerModel>>();
        
        return ResultOf<List<CustomerModel>>.Success(response);
    }
}