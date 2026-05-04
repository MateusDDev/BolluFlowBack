using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Application.Services;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Errors;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.OrderItems.CreateOrderItems;

public class CreateOrderItemsHandler(ApplicationDbContext db) : IRequestHandler<CreateOrderItemsCommand, ResultOf<List<OrderItemModel>>>
{
    public async Task<ResultOf<List<OrderItemModel>>> Handle(CreateOrderItemsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await db.Orders.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);
            if (order is null)
                return ResultOf<List<OrderItemModel>>.NotFound(CommonErrors.OrderNotFound);

            var orderItemsIds = order.Items.Select(x => x.ProductId).ToHashSet();
            if (request.Items.Any(x => orderItemsIds.Contains(x.ProductId)))
                return ResultOf<List<OrderItemModel>>.Conflict(new DomainError("ITEM_ALREADY_ADDED", "Item já adicionado ao pedido"));

            var productIds = request.Items.Select(x => x.ProductId).Distinct().ToList();
            var products = await db.Products.AsNoTracking().Where(x => productIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);

            var components = await db.ProductComponents
                .AsNoTracking()
                .Include(x => x.Recipe)
                .ThenInclude(x => x!.RecipeIngredients)
                .ThenInclude(x => x.Ingredient)
                .Where(x => productIds.Contains(x.ProductId))
                .ToListAsync(cancellationToken);

            var componentCostsByProduct = components
                .GroupBy(x => x.ProductId)
                .ToDictionary(
                    x => x.Key,
                    x => x.Sum(ProductComponentCostService.CalculateComponentCost));

            foreach (var item in request.Items)
            {
                if (!products.TryGetValue(item.ProductId, out var product))
                    return ResultOf<List<OrderItemModel>>.NotFound(CommonErrors.ProductNotFound);

                if (!product.IsActive)
                    return ResultOf<List<OrderItemModel>>.Unprocessable(ProductErrors.ProductInactive);

                var unitCost = componentCostsByProduct.GetValueOrDefault(item.ProductId, 0m);
                order.AddItem(product.Id, item.Quantity, product.FinalPrice, unitCost);
            }

            await db.SaveChangesAsync(cancellationToken);

            var created = await db.OrderItems.Where(x => x.OrderId == request.OrderId).Select(x => new OrderItemModel
            {
                Id = x.Id,
                OrderId = x.OrderId,
                ProductId = x.ProductId,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice,
                TotalPrice = x.UnitPrice * x.Quantity,
                Product = new ProductRefModel { Id = x.ProductId, Name = x.Product.Name }
            }).ToListAsync(cancellationToken);

            return ResultOf<List<OrderItemModel>>.Success(created);
        }
        catch (DomainException e)
        {
            return ResultOf<List<OrderItemModel>>.BadRequest(e.Error);
        }
    }
}
