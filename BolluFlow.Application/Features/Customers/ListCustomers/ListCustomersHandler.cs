using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Customers.ListCustomers;

public class ListCustomersHandler(ApplicationDbContext db, CurrentUser user) : 
    IRequestHandler<ListCustomersQuery, ResultOf<List<CustomerModel>>>
{
    public async Task<ResultOf<List<CustomerModel>>> Handle(ListCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await db.Customers
            .Include(x => x.DeliveryAddress)
            .Where(x => x.UserId == user.Id)
            .ToListAsync(cancellationToken);

        var response = customers.Select(CustomerModel.From).ToList();
        
        return ResultOf<List<CustomerModel>>.Success(response);
    }
}