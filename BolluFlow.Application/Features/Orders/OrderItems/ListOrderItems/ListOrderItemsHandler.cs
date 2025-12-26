using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.OrderItems.ListOrderItems;

public class ListOrderItemsHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<ListOrderItemsQuery, ResultOf<List<OrderItemModel>>>
{
    public async Task<ResultOf<List<OrderItemModel>>> Handle(ListOrderItemsQuery request, CancellationToken cancellationToken)
    {
        var order = await db.Orders
            .Include(x => x.Items)
            .Where(x => x.Customer.UserId == user.Id && x.Id == request.OrderId)
            .FirstOrDefaultAsync(cancellationToken);

        if (order is null)
            return ResultOf<List<OrderItemModel>>.BadRequest(CommonErrors.OrderNotFound);

        return ResultOf<List<OrderItemModel>>.Success(order.Items.Adapt<List<OrderItemModel>>());
    }
}