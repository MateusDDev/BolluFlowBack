using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Customers.DeleteCustomer;

public class DeleteCustomerHandler(ApplicationDbContext db) : IRequestHandler<DeleteCustomerCommand, Result>
{
    public async Task<Result> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await db.Customers.FirstOrDefaultAsync(x => x.Id == request.CustomerId, cancellationToken);

        if (customer is null)
            return Result.NotFound(CommonErrors.CustomerNotFound);
        
        db.Customers.Remove(customer);
        
        await db.SaveChangesAsync(cancellationToken);
        
        return Result.NoContent();
    }
}