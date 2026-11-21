using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Entities;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Customers.UpdateCustomer;

public class UpdateCustomerHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<UpdateCustomerCommand, ResultOf<CustomerModel>>
{
    public async Task<ResultOf<CustomerModel>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var command = request.Body;

        var customer = await db.Customers
            .Include(x => x.DeliveryAddress)
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == user.Id, cancellationToken);

        if (customer is null)
            return ResultOf<CustomerModel>.NotFound(CommonErrors.CustomerNotFound);

        if (!string.IsNullOrWhiteSpace(command.Whatsapp))
            customer.Whatsapp = command.Whatsapp.Trim();

        var address = command.DeliveryAddress is null
            ? customer.DeliveryAddress
            : DeliveryAddress.Create(command.DeliveryAddress.Street, command.DeliveryAddress.Number, command.DeliveryAddress.Neighborhood, command.DeliveryAddress.Complement);

        customer.Update(
            string.IsNullOrWhiteSpace(command.Name) ? customer.Name : command.Name.Trim(),
            address,
            command.Preferences,
            command.Birthday);

        await db.SaveChangesAsync(cancellationToken);

        return ResultOf<CustomerModel>.Success(CustomerModel.From(customer));
    }
}
