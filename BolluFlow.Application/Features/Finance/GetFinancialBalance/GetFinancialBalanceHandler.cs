using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Finance.GetFinancialBalance;

public class GetFinancialBalanceHandler(ApplicationDbContext db)
    : IRequestHandler<GetFinancialBalanceQuery, ResultOf<FinancialBalanceModel>>
{
    public async Task<ResultOf<FinancialBalanceModel>> Handle(GetFinancialBalanceQuery request, CancellationToken cancellationToken)
    {
        var totalIncome = await db.FinancialEntries
            .Where(x => x.Type == FinancialEntryType.Income)
            .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0;

        var totalExpense = await db.FinancialEntries
            .Where(x => x.Type == FinancialEntryType.Expense)
            .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0;

        return ResultOf<FinancialBalanceModel>.Success(new FinancialBalanceModel
        {
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            Balance = totalIncome - totalExpense
        });
    }
}
