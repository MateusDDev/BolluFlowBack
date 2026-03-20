using BolluFlow.Application.Abstractions;
using BolluFlow.Domain.Enums;
using BolluFlow.Domain.Errors;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Finance.DeleteTransaction;

public class DeleteFinancialTransactionHandler(ApplicationDbContext db) : IRequestHandler<DeleteFinancialTransactionCommand, ResultOf<object>>
{
    public async Task<ResultOf<object>> Handle(DeleteFinancialTransactionCommand request, CancellationToken cancellationToken)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        var transaction = await db.FinancialTransactions.FirstOrDefaultAsync(x => x.Id == request.TransactionId && x.DeletedAt == null, cancellationToken);
        if (transaction is null)
            return ResultOf<object>.NotFound(FinanceErrors.TransactionNotFound);

        if (transaction.SourceType == FinancialTransactionSourceType.Sale)
            return ResultOf<object>.Conflict(FinanceErrors.SaleTransactionDeleteBlocked);

        if (transaction.SourceType == FinancialTransactionSourceType.FixedCost)
            return ResultOf<object>.Conflict(FinanceErrors.FixedCostTransactionDeleteBlocked);

        var response = new Dictionary<string, object?>
        {
            ["deleted"] = true,
            ["cascade"] = false,
            ["sourceEntityType"] = transaction.SourceEntityType.ToString().ToLowerInvariant()
        };

        if (transaction.SourceEntityType == FinancialSourceEntityType.Purchase && transaction.SourceEntityId.HasValue)
        {
            var purchase = await db.Purchases.FirstOrDefaultAsync(x => x.Id == transaction.SourceEntityId.Value && x.DeletedAt == null, cancellationToken);
            if (purchase is null)
                return ResultOf<object>.NotFound(FinanceErrors.PurchaseNotFound);

            if (purchase.ItemType == PurchaseItemType.Ingredient)
            {
                var ingredient = await db.Ingredients.FirstOrDefaultAsync(x => x.Id == purchase.ItemId, cancellationToken);
                ingredient?.ConsumeStock(purchase.QuantityInBaseUnit);
            }
            else
            {
                var supply = await db.Supplies.FirstOrDefaultAsync(x => x.Id == purchase.ItemId, cancellationToken);
                supply?.ConsumeStock(purchase.QuantityInBaseUnit);
            }

            purchase.SoftDelete();
            response["cascade"] = true;
            response["sourceEntityType"] = "purchase";
        }

        transaction.SoftDelete();
        await db.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return ResultOf<object>.Success(response);
    }
}
