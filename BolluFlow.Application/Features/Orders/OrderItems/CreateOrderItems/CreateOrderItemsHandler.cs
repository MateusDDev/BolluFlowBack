using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.OrderItems.CreateOrderItems;

public class CreateOrderItemsHandler(ApplicationDbContext db) : IRequestHandler<CreateOrderItemsCommand, ResultOf<List<OrderItemModel>>>
{
    public async Task<ResultOf<List<OrderItemModel>>> Handle(CreateOrderItemsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var requestItems = request.Items;

            var order = await db.Orders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);

            if (order is null)
                return ResultOf<List<OrderItemModel>>.BadRequest(CommonErrors.OrderNotFound);
            
            var orderItemsIds = order.Items.Select(x => x.RecipeId);

            if (requestItems.Any(x => orderItemsIds.Contains(x.RecipeId)))
            {
                return ResultOf<List<OrderItemModel>>.BadRequest(new DomainError(
                    "ITEM_ALREADY_ADDED",
                    "Item já adicionado ao pedido"));
            }

            var recipesIds = requestItems
                .Select(x => x.RecipeId)
                .Distinct()
                .ToList();

            var recipes = await db.Recipes
                .AsNoTracking()
                .Include(x => x.RecipeIngredients)
                .ThenInclude(x => x.Ingredient)
                .Where(x => recipesIds.Contains(x.Id) && x.IsActive)
                .ToDictionaryAsync(x => x.Id, cancellationToken);

            foreach (var item in requestItems)
            {
                if (!recipes.TryGetValue(item.RecipeId, out var recipe))
                    return ResultOf<List<OrderItemModel>>.BadRequest(CommonErrors.RecipeNotFound);

                order.AddItem(item.RecipeId, item.Quantity, recipe.FinalPrice, recipe.CalculateUnitCost());
            }

            await db.SaveChangesAsync(cancellationToken);

            return ResultOf<List<OrderItemModel>>.Success(order.Items.Adapt<List<OrderItemModel>>());
        }
        catch (DomainException e)
        {
            return ResultOf<List<OrderItemModel>>.BadRequest(e.Error);
        }
    }
}