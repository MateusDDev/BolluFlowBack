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
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var order = await db.Orders
                .Include(x => x.Items)
                .Include(x => x.Customer)
                .FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);

            if (order is null)
                return Result.NotFound(CommonErrors.OrderNotFound);

            if (order.Status != OrderStatus.Pending)
                return Result.Unprocessable(OrderErrors.InvalidStartTransition);

            if (order.Items.Count == 0)
                return Result.Unprocessable(new DomainError("ORDER_WITHOUT_ITEMS", "Pedido sem itens não pode ser iniciado"));

            var productIds = order.Items.Select(x => x.ProductId).Distinct().ToList();
            var products = await db.Products
                .Where(x => productIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);

            var skipStockCheck = await db.Users
                .Where(x => x.Id == order.Customer.UserId)
                .Select(x => x.SkipStockCheck)
                .FirstOrDefaultAsync(cancellationToken);

            if (!skipStockCheck)
            {
                var insufficientProducts = order.Items
                    .Select(item => new
                    {
                        Item = item,
                        Product = products.GetValueOrDefault(item.ProductId)
                    })
                    .Where(x => x.Product is null || x.Product.StockQuantity < x.Item.Quantity)
                    .Select(x => new
                    {
                        productId = x.Item.ProductId,
                        productName = x.Product?.Name ?? "Produto não encontrado",
                        required = x.Item.Quantity,
                        available = x.Product?.StockQuantity ?? 0
                    })
                    .ToList();

                if (insufficientProducts.Count > 0)
                {
                    return Result.Unprocessable(
                        new DomainError("INSUFFICIENT_STOCK", "Estoque insuficiente para iniciar a produção do pedido"),
                        new { insufficientProducts });
                }
            }

            order.Status = OrderStatus.InProduction;

            if (!skipStockCheck)
            {
                foreach (var item in order.Items)
                {
                    var product = products[item.ProductId];
                    product.ConsumeStock(item.Quantity);
                }
            }

            await db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch (DomainException e)
        {
            await tx.RollbackAsync(cancellationToken);
            return Result.BadRequest(e.Error);
        }
    }
}
