using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.StartOrder;

public class StartOrderHandler(ApplicationDbContext db) : IRequestHandler<StartOrderCommand, Result>
{
    public async Task<Result> Handle(StartOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await db.Orders
            .Include(x => x.Items)
            .ThenInclude(x => x.Recipe)
            .ThenInclude(x => x.RecipeIngredients)
            .ThenInclude(x => x.Ingredient)
            .FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);

        if (order is null)
            return Result.NotFound(CommonErrors.OrderNotFound);

        foreach (var item in order.Items)
        {
            foreach (var ri in item.Recipe.RecipeIngredients)
            {
                ri.Ingredient.ConsumeStock(ri.QuantityUsed);
            }
        }

        order.Status = OrderStatus.InProduction;
        
        await db.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}