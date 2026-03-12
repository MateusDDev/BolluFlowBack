using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Enums;
using BolluFlow.Domain.Errors;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.StartOrder;

public class StartOrderHandler(ApplicationDbContext db) : IRequestHandler<StartOrderCommand, Result>
{
    public async Task<Result> Handle(StartOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await db.Orders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);

            if (order is null)
                return Result.NotFound(CommonErrors.OrderNotFound);

            if (order.Status != OrderStatus.Pending)
                return Result.Unprocessable(OrderErrors.InvalidStartTransition);

            if (order.Items.Count == 0)
                return Result.Unprocessable(new DomainError("ORDER_WITHOUT_ITEMS", "Pedido sem itens não pode ser iniciado"));

            var productIds = order.Items.Select(x => x.ProductId).Distinct().ToList();
            var products = await db.Products
                .Include(x => x.Components)
                .Where(x => productIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);

            foreach (var item in order.Items)
            {
                if (!products.TryGetValue(item.ProductId, out var product))
                    return Result.NotFound(CommonErrors.ProductNotFound);

                if (product.StockQuantity < item.Quantity)
                    return Result.Unprocessable(ProductErrors.StockInsufficient(product.Name));

                foreach (var component in product.Components)
                {
                    var total = component.QuantityUsed * item.Quantity;

                    if (component.ComponentType == ProductComponentType.Ingredient && component.IngredientId.HasValue)
                    {
                        var ingredient = await db.Ingredients.FirstOrDefaultAsync(x => x.Id == component.IngredientId.Value, cancellationToken);
                        if (ingredient is null)
                            return Result.NotFound(CommonErrors.IngredientNotFound);

                        if (ingredient.StockQuantity < total)
                            return Result.Unprocessable(IngredientErrors.StockInsufficient(ingredient.Name));
                    }

                    if (component.ComponentType == ProductComponentType.Supply && component.SupplyId.HasValue)
                    {
                        var supply = await db.Supplies.FirstOrDefaultAsync(x => x.Id == component.SupplyId.Value, cancellationToken);
                        if (supply is null)
                            return Result.NotFound(CommonErrors.SupplyNotFound);

                        if (supply.StockQuantity < total)
                            return Result.Unprocessable(SupplyErrors.StockInsufficient(supply.Name));
                    }

                    if (component.ComponentType == ProductComponentType.Recipe && component.RecipeId.HasValue)
                    {
                        var recipeIngredients = await db.RecipeIngredients.Include(x => x.Ingredient)
                            .Where(x => x.RecipeId == component.RecipeId.Value)
                            .ToListAsync(cancellationToken);

                        foreach (var recipeIngredient in recipeIngredients)
                        {
                            var required = recipeIngredient.QuantityUsed * total;
                            if (recipeIngredient.Ingredient.StockQuantity < required)
                                return Result.Unprocessable(IngredientErrors.StockInsufficient(recipeIngredient.Ingredient.Name));
                        }
                    }
                }
            }

            order.Status = OrderStatus.InProduction;

            await db.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (DomainException e)
        {
            return Result.BadRequest(e.Error);
        }
    }
}
