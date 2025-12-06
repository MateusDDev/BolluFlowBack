using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Entities;
using BolluFlow.Infrastructure.Persistence;
using Mapster;
using MediatR;

namespace BolluFlow.Application.Features.Customers.Create;

public class CreateCustomerHandler(ApplicationDbContext db) : IRequestHandler<CreateCustomerCommand, ResultOf<CustomerModel>>
{
    public async Task<ResultOf<CustomerModel>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var newCustomer = request.Adapt<Customer>();
        
        db.Customers.Add(newCustomer);
        
        await db.SaveChangesAsync(cancellationToken);
        
        var result = newCustomer.Adapt<CustomerModel>();
        
        return ResultOf<CustomerModel>.Success(result);
    }
}