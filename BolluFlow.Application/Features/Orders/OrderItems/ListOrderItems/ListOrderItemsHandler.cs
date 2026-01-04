using BolluFlow.Application.Abstractions;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.OrderItems.ListOrderItems;

public class ListOrderItemsHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<ListOrderItemsQuery, ResultOf<List<ListOrderItemsResult>>>
{
    public async Task<ResultOf<List<ListOrderItemsResult>>> Handle(ListOrderItemsQuery request, CancellationToken cancellationToken)
    {
        var orderItems = await db.OrderItems
            .Where(x => x.Order.Customer.UserId == user.Id && x.OrderId == request.OrderId)
            .Select(x => new ListOrderItemsResult
            {
                Id = x.Id,
                OrderId = x.OrderId,
                Quantity = x.Quantity,
                Recipe = new ListOrderItemsResultRecipeModel
                {
                    Id = x.RecipeId,
                    Name = x.Recipe.Name
                },
                UnitPrice = x.UnitPrice,
                TotalPrice = x.UnitPrice * x.Quantity
            })
            .ToListAsync(cancellationToken);

        return ResultOf<List<ListOrderItemsResult>>.Success(orderItems);
    }
}