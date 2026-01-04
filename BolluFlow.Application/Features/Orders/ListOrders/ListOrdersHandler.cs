using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.ListOrders;

public class ListOrdersHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<ListOrdersQuery, ResultOf<List<OrderModel>>>
{
    public async Task<ResultOf<List<OrderModel>>> Handle(ListOrdersQuery request, CancellationToken cancellationToken)
    {
        var recipes = await db.Orders
            .Include(x => x.Customer)
            .Where(x => x.Customer.UserId == user.Id)
            .ToListAsync(cancellationToken);

        return ResultOf<List<OrderModel>>.Success(recipes.Adapt<List<OrderModel>>());
    }
}