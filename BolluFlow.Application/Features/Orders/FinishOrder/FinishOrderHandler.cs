using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.FinishOrder;

public class FinishOrderHandler(ApplicationDbContext db) : IRequestHandler<FinishOrderCommand, Result>
{
    public async Task<Result> Handle(FinishOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await db.Orders
                .FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);

            if (order is null)
                return Result.NotFound(CommonErrors.OrderNotFound);

            order.Status = OrderStatus.Ready;

            await db.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (DomainException e)
        {
            return Result.BadRequest(e.Error);
        }
    }
}