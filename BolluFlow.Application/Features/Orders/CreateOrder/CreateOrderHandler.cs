using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.CreateOrder;

public class CreateOrderHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<CreateOrderCommand, ResultOf<OrderModel>>
{
    public async Task<ResultOf<OrderModel>> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var customerExists = await db.Customers
                .AnyAsync(x => x.Id == request.CustomerId, cancellationToken);

            if (!customerExists)
                return ResultOf<OrderModel>.NotFound(CommonErrors.CustomerNotFound);
            
            var lastNumber = await db.Orders
                .Where(x => x.Customer.UserId == user.Id)
                .MaxAsync(x => (int?)x.Number, cancellationToken) ?? 0;
            
            var order = Order.Create(
                request.CustomerId,
                lastNumber + 1,
                request.Note,
                request.DeliveryDate,
                request.PaymentMethod);
            
            db.Orders.Add(order);
            
            await db.SaveChangesAsync(cancellationToken);

            return ResultOf<OrderModel>.Success(OrderModel.From(order));
        }
        catch (DomainException e)
        {
            return ResultOf<OrderModel>.BadRequest(e.Error);
        }
    }

}
