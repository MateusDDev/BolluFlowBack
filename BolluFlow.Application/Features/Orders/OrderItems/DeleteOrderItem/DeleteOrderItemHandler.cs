using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.OrderItems.DeleteOrderItem;

public class DeleteOrderItemHandler(ApplicationDbContext db) : IRequestHandler<DeleteOrderItemCommand, Result>
{
    public async Task<Result> Handle(DeleteOrderItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await db.Orders
                .FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);

            if (order is null)
                return Result.BadRequest(CommonErrors.OrderNotFound);

            order.RemoveItem(request.ItemId);

            await db.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (DomainException e)
        {
            return Result.BadRequest(e.Error);
        }
    }
}