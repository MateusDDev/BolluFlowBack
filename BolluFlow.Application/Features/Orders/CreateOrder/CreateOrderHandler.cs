using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.CreateOrder;

public class CreateOrderHandler(ApplicationDbContext db) : IRequestHandler<CreateOrderCommand, ResultOf<OrderModel>>
{
    public async Task<ResultOf<OrderModel>> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var customerExists = await db.Customers
                .AnyAsync(x => x.Id == request.CustomerId, cancellationToken);

            if (!customerExists)
                return ResultOf<OrderModel>.NotFound(CommonErrors.CustomerNotFound);
            
            var order = Order.Create(request.CustomerId, request.Note, request.DeliveryDate);
            
            var recipeIds = request.Items
                .Select(x => x.RecipeId)
                .Distinct()
                .ToList();

            var recipes = await db.Recipes
                .Include(x => x.RecipeIngredients)
                .ThenInclude(x => x.Ingredient)
                .Where(x => recipeIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

            var recipesById = recipes.ToDictionary(x => x.Id);

            foreach (var item in request.Items)
            {
                if (!recipesById.TryGetValue(item.RecipeId, out var recipe))
                    return ResultOf<OrderModel>.NotFound(CommonErrors.RecipeNotFound);
                
                order.AddItem(recipe, item.Quantity);
            }
            
            db.Orders.Add(order);
            
            await db.SaveChangesAsync(cancellationToken);

            return ResultOf<OrderModel>.Success(order.Adapt<OrderModel>());
        }
        catch (DomainException e)
        {
            return ResultOf<OrderModel>.BadRequest(e.Error);
        }
    }

}