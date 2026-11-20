using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Enums;
using BolluFlow.Domain.Errors;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Orders.StartOrder;

public class StartOrderHandler(ApplicationDbContext db, CurrentUser user) : IRequestHandler<StartOrderCommand, Result>
{
    public async Task<Result> Handle(StartOrderCommand request, CancellationToken cancellationToken)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var order = await db.Orders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == request.OrderId && x.Customer.UserId == user.Id, cancellationToken);

            if (order is null)
                return Result.NotFound(CommonErrors.OrderNotFound);

            if (order.Status != OrderStatus.Pending)
                return Result.Unprocessable(OrderErrors.InvalidStartTransition);

            if (order.Items.Count == 0)
                return Result.Unprocessable(new DomainError("ORDER_WITHOUT_ITEMS", "Pedido sem itens não pode ser iniciado"));

            var allowNegativeStock = await db.Users
                .Where(x => x.Id == user.Id)
                .Select(x => x.AllowNegativeStock || (x.EnabledModules & AppModule.Stock) == AppModule.None)
                .FirstOrDefaultAsync(cancellationToken);

            var productIds = order.Items.Select(x => x.ProductId).Distinct().ToList();
            var products = await db.Products
                .Where(x => productIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);

            var insufficientProducts = order.Items
                .Select(item => new
                {
                    Item = item,
                    Product = products.GetValueOrDefault(item.ProductId)
                })
                .Where(x => x.Product is null || !x.Product.HasEnoughStock(x.Item.Quantity, allowNegativeStock))
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

            order.Status = OrderStatus.InProduction;

            foreach (var item in order.Items)
                products[item.ProductId].ConsumeStock(item.Quantity, allowNegativeStock);

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
