using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.DeliverOrder;

public class DeliverOrderHandler(ApplicationDbContext db) : IRequestHandler<DeliverOrderCommand, Result>
{
    public async Task<Result> Handle(DeliverOrderCommand request, CancellationToken cancellationToken)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var order = await db.Orders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);

            if (order is null)
                return Result.NotFound(CommonErrors.OrderNotFound);

            var productIds = order.Items.Select(x => x.ProductId).Distinct().ToList();
            var products = await db.Products.Include(x => x.Components).Where(x => productIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, cancellationToken);

            foreach (var item in order.Items)
            {
                if (!products.TryGetValue(item.ProductId, out var product))
                    return Result.NotFound(CommonErrors.ProductNotFound);

                product.ConsumeStock(item.Quantity);

                foreach (var component in product.Components)
                {
                    var total = component.QuantityUsed * item.Quantity;
                    if (component.ComponentType == ProductComponentType.Ingredient && component.IngredientId.HasValue)
                    {
                        var ingredient = await db.Ingredients.FirstOrDefaultAsync(x => x.Id == component.IngredientId.Value, cancellationToken);
                        ingredient?.ConsumeStock(total);
                    }

                    if (component.ComponentType == ProductComponentType.Supply && component.SupplyId.HasValue)
                    {
                        var supply = await db.Supplies.FirstOrDefaultAsync(x => x.Id == component.SupplyId.Value, cancellationToken);
                        supply?.ConsumeStock(total);
                    }

                    if (component.ComponentType == ProductComponentType.Recipe && component.RecipeId.HasValue)
                    {
                        var recipeIngredients = await db.RecipeIngredients.Include(x => x.Ingredient)
                            .Where(x => x.RecipeId == component.RecipeId.Value).ToListAsync(cancellationToken);

                        foreach (var recipeIngredient in recipeIngredients)
                            recipeIngredient.Ingredient.ConsumeStock(recipeIngredient.QuantityUsed * total);
                    }
                }
            }

            order.Status = OrderStatus.Delivered;
            await db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch (DomainException e)
        {
            await tx.RollbackAsync(cancellationToken);
            return Result.Unprocessable(e.Error);
        }
    }
}
