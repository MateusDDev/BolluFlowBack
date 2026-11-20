using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.ListOrders;

public class ListOrdersHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<ListOrdersQuery, ResultOf<List<OrderModel>>>
{
    public async Task<ResultOf<List<OrderModel>>> Handle(ListOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await db.Orders
            .Include(x => x.Customer)
            .ThenInclude(x => x!.DeliveryAddress)
            .Include(x => x.Items)
            .Where(x => x.UserId == user.Id)
            .ToListAsync(cancellationToken);

        var response = orders.Select(OrderModel.From).ToList();

        return ResultOf<List<OrderModel>>.Success(response);
    }
}
