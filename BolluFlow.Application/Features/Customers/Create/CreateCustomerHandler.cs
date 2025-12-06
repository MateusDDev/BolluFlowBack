using BolluFlow.Application.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Infrastructure.Persistence;
using MediatR;

namespace BolluFlow.Application.Features.Customers.Create;

public class CreateCustomerHandler(ApplicationDbContext db) : IRequestHandler<CreateCustomerCommand, ResultOf<Customer>>
{
    public async Task<ResultOf<Customer>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var newCustomer = new Customer
        {
            Name = request.Name,
            Whatsapp = request.Whatsapp,
            Preferences = request.Preferences,
            DeliveryAddress = new DeliveryAddress
            {
                Number = request.DeliveryAddress.Number,
                Neighborhood = request.DeliveryAddress.Neighborhood,
                Street = request.DeliveryAddress.Street
            }
        };
        
        db.Customers.Add(newCustomer);
        
        await db.SaveChangesAsync(cancellationToken);
        
        return ResultOf<Customer>.Success(newCustomer);
    }
}