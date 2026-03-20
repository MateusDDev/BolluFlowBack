using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Finance.Shared;
using BolluFlow.Domain.Errors;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Finance.ListTransactions;

public class ListFinancialTransactionsHandler(ApplicationDbContext db) : IRequestHandler<ListFinancialTransactionsQuery, ResultOf<List<FinancialTransactionDto>>>
{
    public async Task<ResultOf<List<FinancialTransactionDto>>> Handle(ListFinancialTransactionsQuery request, CancellationToken cancellationToken)
    {
        if (!FinanceMonth.TryParseMonth(request.Month, out _))
            return ResultOf<List<FinancialTransactionDto>>.BadRequest(FinanceErrors.InvalidMonth);

        var transactions = await db.FinancialTransactions
            .Where(x => x.DeletedAt == null && x.Month == request.Month)
            .OrderByDescending(x => x.EventDate)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        var fixedCosts = await db.FixedCosts
            .Where(x => x.DeletedAt == null && string.Compare(x.StartMonth, request.Month, StringComparison.Ordinal) <= 0 && (x.EndMonth == null || string.Compare(x.EndMonth, request.Month, StringComparison.Ordinal) >= 0))
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        var result = transactions.Select(x => x.ToDto())
            .Concat(fixedCosts.Select(x => x.ToFixedCostTransactionDto(request.Month)))
            .OrderByDescending(x => x.EventDate)
            .ThenByDescending(x => x.IsAutomatic)
            .ToList();

        return ResultOf<List<FinancialTransactionDto>>.Success(result);
    }
}
