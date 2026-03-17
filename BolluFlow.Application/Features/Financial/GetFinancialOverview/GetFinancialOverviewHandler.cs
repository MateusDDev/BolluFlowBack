using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Enums;
using BolluFlow.Domain.Errors;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Financial.GetFinancialOverview;

public class GetFinancialOverviewHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<GetFinancialOverviewQuery, ResultOf<FinancialOverviewModel>>
{
    public async Task<ResultOf<FinancialOverviewModel>> Handle(GetFinancialOverviewQuery request, CancellationToken cancellationToken)
    {
        if (!DateOnly.TryParse($"{request.Month}-01", out var monthDate))
            return ResultOf<FinancialOverviewModel>.BadRequest(FinancialErrors.InvalidMonthFormat);

        await FinancialSalesEntrySync.EnsureSalesEntries(db, user.Id, cancellationToken);

        var startMonth = new DateOnly(monthDate.Year, monthDate.Month, 1);
        var endMonth = startMonth.AddMonths(1).AddDays(-1);

        var entries = await db.FinancialEntries
            .Where(x => x.UserId == user.Id)
            .ToListAsync(cancellationToken);

        var totalIncomesPaid = entries.Where(x => x.Type == FinancialEntryType.Income && x.Status == FinancialEntryStatus.Paid).Sum(x => x.Amount);
        var totalExpensesPaid = entries.Where(x => x.Type == FinancialEntryType.Expense && x.Status == FinancialEntryStatus.Paid).Sum(x => x.Amount);
        var pendingToReceive = entries.Where(x => x.Type == FinancialEntryType.Income && x.Status == FinancialEntryStatus.Pending).Sum(x => x.Amount);
        var pendingToPay = entries.Where(x => x.Type == FinancialEntryType.Expense && x.Status == FinancialEntryStatus.Pending).Sum(x => x.Amount);
        var fixedCostsInMonth = entries
            .Where(x => x.IsFixed && x.Type == FinancialEntryType.Expense && x.Date >= startMonth && x.Date <= endMonth)
            .Sum(x => x.Amount);

        var currentBalance = totalIncomesPaid - totalExpensesPaid;
        var forecastBalance = currentBalance + pendingToReceive - pendingToPay;

        return ResultOf<FinancialOverviewModel>.Success(new FinancialOverviewModel
        {
            CurrentBalance = currentBalance,
            TotalIncomesPaid = totalIncomesPaid,
            TotalExpensesPaid = totalExpensesPaid,
            PendingToReceive = pendingToReceive,
            PendingToPay = pendingToPay,
            ForecastBalance = forecastBalance,
            FixedCostsInMonth = fixedCostsInMonth
        });
    }
}
