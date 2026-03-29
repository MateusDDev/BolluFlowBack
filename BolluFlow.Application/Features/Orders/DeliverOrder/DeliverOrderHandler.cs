using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;
using BolluFlow.Domain.Errors;
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

            if (order.Status != OrderStatus.Ready)
                return Result.Unprocessable(OrderErrors.InvalidDeliverTransition);

            var productIds = order.Items.Select(x => x.ProductId).Distinct().ToList();
            var products = await db.Products
                .Include(x => x.Components)
                .Where(x => productIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);

            var ingredientRequirements = new Dictionary<long, decimal>();
            var supplyRequirements = new Dictionary<long, decimal>();

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
                        ingredientRequirements[component.IngredientId.Value] = ingredientRequirements.GetValueOrDefault(component.IngredientId.Value) + total;
                    }

                    if (component.ComponentType == ProductComponentType.Supply && component.SupplyId.HasValue)
                    {
                        supplyRequirements[component.SupplyId.Value] = supplyRequirements.GetValueOrDefault(component.SupplyId.Value) + total;
                    }

                    if (component.ComponentType == ProductComponentType.Recipe && component.RecipeId.HasValue)
                    {
                        var recipeIngredients = await db.RecipeIngredients
                            .Include(x => x.Ingredient)
                            .Where(x => x.RecipeId == component.RecipeId.Value)
                            .ToListAsync(cancellationToken);

                        if (recipeIngredients.Count == 0)
                            return Result.Unprocessable(new DomainError("RECIPE_WITHOUT_INGREDIENTS", "Receita da composição não possui ingredientes"));

                        foreach (var recipeIngredient in recipeIngredients)
                        {
                            var required = recipeIngredient.QuantityUsed * total;
                            ingredientRequirements[recipeIngredient.IngredientId] = ingredientRequirements.GetValueOrDefault(recipeIngredient.IngredientId) + required;
                        }
                    }
                }
            }

            if (ingredientRequirements.Count > 0)
            {
                var ingredientIds = ingredientRequirements.Keys.ToList();
                var ingredients = await db.Ingredients.Where(x => ingredientIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, cancellationToken);

                foreach (var (ingredientId, quantity) in ingredientRequirements)
                {
                    if (!ingredients.TryGetValue(ingredientId, out var ingredient))
                        return Result.NotFound(CommonErrors.IngredientNotFound);

                    ingredient.ConsumeStock(quantity);

                    var movementEventKey = $"order_consumption:ingredient:{ingredientId}:order:{order.Id}";
                    var existingMovement = await db.StockMovements.FirstOrDefaultAsync(x => x.EventKey == movementEventKey, cancellationToken);

                    if (existingMovement is null)
                    {
                        db.StockMovements.Add(StockMovement.Create(
                            StockItemType.Ingredient,
                            ingredientId,
                            StockMovementType.Consumption,
                            quantity,
                            ingredient.UnitCost,
                            $"Consumo automático do pedido #{order.Number}",
                            "order",
                            order.Id,
                            movementEventKey));
                    }
                }
            }

            if (supplyRequirements.Count > 0)
            {
                var supplyIds = supplyRequirements.Keys.ToList();
                var supplies = await db.Supplies.Where(x => supplyIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, cancellationToken);

                foreach (var (supplyId, quantity) in supplyRequirements)
                {
                    if (!supplies.TryGetValue(supplyId, out var supply))
                        return Result.NotFound(CommonErrors.SupplyNotFound);

                    supply.ConsumeStock(quantity);

                    var movementEventKey = $"order_consumption:supply:{supplyId}:order:{order.Id}";
                    var existingMovement = await db.StockMovements.FirstOrDefaultAsync(x => x.EventKey == movementEventKey, cancellationToken);

                    if (existingMovement is null)
                    {
                        db.StockMovements.Add(StockMovement.Create(
                            StockItemType.Supply,
                            supplyId,
                            StockMovementType.Consumption,
                            quantity,
                            supply.UnitCost,
                            $"Consumo automático do pedido #{order.Number}",
                            "order",
                            order.Id,
                            movementEventKey));
                    }
                }
            }

            order.Status = OrderStatus.Delivered;

            var financialEventKey = $"order_delivered:{order.Id}";
            var existingFinancial = await db.FinancialEntries
                .FirstOrDefaultAsync(x => x.EventKey == financialEventKey, cancellationToken);

            if (existingFinancial is null)
            {
                var orderTotal = decimal.Round(order.Items.Sum(x => x.Quantity * x.UnitPrice), 2, MidpointRounding.AwayFromZero);

                db.FinancialEntries.Add(FinancialEntry.Create(
                    FinancialEntryType.Saida,
                    orderTotal,
                    $"Saída automática do pedido #{order.Number}",
                    FinancialOriginType.Pedido,
                    order.Id,
                    "order_delivered",
                    financialEventKey));
            }

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
