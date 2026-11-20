using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.GetOrderDetails;

public class GetOrderDetailsHandler(ApplicationDbContext db)
    : IRequestHandler<GetOrderDetailsQuery, ResultOf<OrderModel>>
{
    public async Task<ResultOf<OrderModel>> Handle(GetOrderDetailsQuery request, CancellationToken cancellationToken)
    {
        var order = await db.Orders
            .Include(x => x.Customer)
            .ThenInclude(x => x.DeliveryAddress)
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);

        if (order is null)
            return ResultOf<OrderModel>.BadRequest(CommonErrors.OrderNotFound);

        return ResultOf<OrderModel>.Success(OrderModel.From(order));
    }
}
