using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.CancelOrder;

public class CancelOrderHandler(ApplicationDbContext db) : IRequestHandler<CancelOrderCommand, Result>
{
    public async Task<Result> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await db.Orders
            .Include(x => x.Items)
            .ThenInclude(x => x.Recipe)
            .ThenInclude(x => x.RecipeIngredients)
            .ThenInclude(x => x.Ingredient)
            .FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);

        if (order is null)
            return Result.NotFound(CommonErrors.OrderNotFound);

        if (order.Status is OrderStatus.Canceled or OrderStatus.Delivered)
        {
            return Result.BadRequest(new DomainError("ORDER_NOT_POSSIBLE_TO_CANCEL",
                "Não é possível cancelar pedidos já entregues ou já cancelados"));
        }
        
        order.Status = OrderStatus.Canceled;
        
        foreach (var item in order.Items)
        {
            foreach (var ri in item.Recipe.RecipeIngredients)
            {
                ri.Ingredient.AddStock(ri.QuantityUsed * item.Quantity);
            }
        }
        
        await db.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}