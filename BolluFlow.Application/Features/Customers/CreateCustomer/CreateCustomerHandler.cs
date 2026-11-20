using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Entities;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;

namespace BolluFlow.Application.Features.Customers.CreateCustomer;

public class CreateCustomerHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<CreateCustomerCommand, ResultOf<CustomerModel>>
{
    public async Task<ResultOf<CustomerModel>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var deliveryAddress = request.DeliveryAddress;

        var customer = Customer.Create(
            user.Id,
            request.Name,
            request.Whatsapp,
            DeliveryAddress.Create(deliveryAddress.Street, deliveryAddress.Number, deliveryAddress.Neighborhood),
            request.Preferences,
            request.Birthday);
        
        db.Customers.Add(customer);
        
        await db.SaveChangesAsync(cancellationToken);
        
        var result = CustomerModel.From(customer);
        
        return ResultOf<CustomerModel>.Success(result);
    }
}