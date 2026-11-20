using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.FinishOrder;

public class FinishOrderHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<FinishOrderCommand, Result>
{
    public async Task<Result> Handle(FinishOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await db.Orders
            .FirstOrDefaultAsync(x => x.Id == request.OrderId && x.UserId == user.Id, cancellationToken);

        if (order is null)
            return Result.NotFound(CommonErrors.OrderNotFound);

        try
        {
            order.FinishProduction();
        }
        catch (DomainException e)
        {
            return Result.Unprocessable(e.Error);
        }

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
