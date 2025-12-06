using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Customers.Update;

public class UpdateCustomerHandler(ApplicationDbContext db) : IRequestHandler<UpdateCustomerCommand, ResultOf<CustomerModel>>
{
    
    public async Task<ResultOf<CustomerModel>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await db.Customers
            .Include(x => x.DeliveryAddress)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (customer is null)
            return ResultOf<CustomerModel>.NotFound("Cliente não encontrado");
        
        request.Body.Adapt(customer);
        
        await db.SaveChangesAsync(cancellationToken);
        
        var result = customer.Adapt<CustomerModel>();
        
        return ResultOf<CustomerModel>.Success(result);
    }
}