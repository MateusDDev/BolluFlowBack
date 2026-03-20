using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Finance.Shared;
using BolluFlow.Domain.Enums;
using BolluFlow.Domain.Errors;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Finance.GetOverview;

public class GetFinanceOverviewHandler(ApplicationDbContext db) : IRequestHandler<GetFinanceOverviewQuery, ResultOf<FinanceOverviewDto>>
{
    public async Task<ResultOf<FinanceOverviewDto>> Handle(GetFinanceOverviewQuery request, CancellationToken cancellationToken)
    {
        if (!FinanceMonth.TryParseMonth(request.Month, out var monthDate))
            return ResultOf<FinanceOverviewDto>.BadRequest(FinanceErrors.InvalidMonth);

        var manualAndAutomaticTransactions = await db.FinancialTransactions
            .Where(x => x.DeletedAt == null && string.Compare(x.Month, request.Month, StringComparison.Ordinal) <= 0)
            .OrderByDescending(x => x.EventDate)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        var fixedCosts = await db.FixedCosts
            .Where(x => x.DeletedAt == null && string.Compare(x.StartMonth, request.Month, StringComparison.Ordinal) <= 0 && (x.EndMonth == null || string.Compare(x.EndMonth, request.Month, StringComparison.Ordinal) >= 0))
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        var monthTransactions = manualAndAutomaticTransactions.Where(x => x.Month == request.Month).Select(x => x.ToDto()).ToList();
        monthTransactions.AddRange(fixedCosts.Select(x => x.ToFixedCostTransactionDto(request.Month)));

        var incomeTotal = monthTransactions.Where(x => x.Type == "income").Sum(x => x.Amount);
        var expenseTotal = monthTransactions.Where(x => x.Type == "expense").Sum(x => x.Amount);

        decimal cumulativeIncome = manualAndAutomaticTransactions.Where(x => x.Type == FinancialTransactionType.Income).Sum(x => x.Amount);
        decimal cumulativeExpense = manualAndAutomaticTransactions.Where(x => x.Type == FinancialTransactionType.Expense).Sum(x => x.Amount);

        var earliestMonth = manualAndAutomaticTransactions.Select(x => x.Month)
            .Concat(await db.FixedCosts.Where(x => x.DeletedAt == null).Select(x => x.StartMonth).ToListAsync(cancellationToken))
            .OrderBy(x => x)
            .FirstOrDefault() ?? request.Month;

        if (!FinanceMonth.TryParseMonth(earliestMonth, out var startMonth))
            startMonth = monthDate;

        foreach (var cursor in FinanceMonth.EnumerateMonths(startMonth, monthDate))
        {
            var currentMonth = FinanceMonth.Format(cursor);
            cumulativeExpense += await db.FixedCosts
                .Where(x => x.DeletedAt == null && string.Compare(x.StartMonth, currentMonth, StringComparison.Ordinal) <= 0 && (x.EndMonth == null || string.Compare(x.EndMonth, currentMonth, StringComparison.Ordinal) >= 0))
                .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m;
        }

        var recentTransactions = monthTransactions
            .OrderByDescending(x => x.EventDate)
            .ThenByDescending(x => x.IsAutomatic)
            .Take(10)
            .ToList();

        return ResultOf<FinanceOverviewDto>.Success(new FinanceOverviewDto
        {
            Month = request.Month,
            IncomeTotal = incomeTotal,
            ExpenseTotal = expenseTotal,
            MonthBalance = incomeTotal - expenseTotal,
            ProjectedBalance = cumulativeIncome - cumulativeExpense,
            RecentTransactions = recentTransactions
        });
    }
}
