using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.DeliverOrder;

public class DeliverOrderHandler(ApplicationDbContext db) : IRequestHandler<DeliverOrderCommand, Result>
{
    public async Task<Result> Handle(DeliverOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await db.Orders
            .FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);

        if (order is null)
            return Result.NotFound(CommonErrors.OrderNotFound);

        order.Status = OrderStatus.Delivered;
        
        await db.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}