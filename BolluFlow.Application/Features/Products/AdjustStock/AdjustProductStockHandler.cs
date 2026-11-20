using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Errors;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Products.AdjustStock;

public class AdjustProductStockHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<AdjustProductStockCommand, ResultOf<ProductStockAdjustmentModel>>
{
    public async Task<ResultOf<ProductStockAdjustmentModel>> Handle(AdjustProductStockCommand request, CancellationToken cancellationToken)
    {
        if (request.Body.Quantity <= 0)
            return ResultOf<ProductStockAdjustmentModel>.BadRequest(new DomainError("PRODUCT_STOCK_QUANTITY_INVALID", "quantity must be greater than zero"));

        if (string.IsNullOrWhiteSpace(request.Body.Reason) || request.Body.Reason.Trim().Length < 3)
            return ResultOf<ProductStockAdjustmentModel>.BadRequest(new DomainError("PRODUCT_STOCK_ADJUSTMENT_REASON_INVALID", "reason must have at least 3 characters"));

        var direction = request.Body.Direction.Trim().ToLowerInvariant();
        if (direction is not ("add" or "remove"))
            return ResultOf<ProductStockAdjustmentModel>.BadRequest(new DomainError("PRODUCT_STOCK_ADJUSTMENT_DIRECTION_INVALID", "direction must be 'add' or 'remove'"));

        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        try
        {

            var product = await db.Products.FirstOrDefaultAsync(x => x.Id == request.ProductId && x.UserId == user.Id, cancellationToken);
            if (product is null)
                return ResultOf<ProductStockAdjustmentModel>.NotFound(CommonErrors.ProductNotFound);

            product.SetTrackStock(true);

            if (direction == "add")
                product.AddStock(request.Body.Quantity);
            else
                product.ConsumeStock(request.Body.Quantity);

            await db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return ResultOf<ProductStockAdjustmentModel>.Success(new ProductStockAdjustmentModel
            {
                ProductId = product.Id,
                Quantity = request.Body.Quantity,
                Direction = direction,
                Reason = request.Body.Reason.Trim(),
                StockQuantity = product.StockQuantity,
                CreatedAt = DateTime.UtcNow
            });
        }
        catch (DomainException e)
        {
            await tx.RollbackAsync(cancellationToken);
            return ResultOf<ProductStockAdjustmentModel>.Unprocessable(e.Error);
        }
    }
}
