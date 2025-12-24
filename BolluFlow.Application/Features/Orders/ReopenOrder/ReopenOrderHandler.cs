using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.ReopenOrder;

public class ReopenOrderHandler(ApplicationDbContext db) : IRequestHandler<ReopenOrderCommand, Result>
{
    public async Task<Result> Handle(ReopenOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await db.Orders
            .FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);

        if (order is null)
            return Result.NotFound(CommonErrors.OrderNotFound);

        if (order.Status != OrderStatus.Canceled)
        {
            return Result.BadRequest(new DomainError("ORDER_NOT_CANCELED",
                "Apenas pedidos cancelados podem ser reabertos"));
        }
        
        order.Status = OrderStatus.Pending;
        
        await db.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}