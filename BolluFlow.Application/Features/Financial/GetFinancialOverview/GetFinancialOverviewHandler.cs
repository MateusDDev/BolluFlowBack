using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Financial;
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
        if (!FinancialMonth.TryParse(request.Month, out var month))
            return ResultOf<FinancialOverviewModel>.BadRequest(FinancialErrors.InvalidMonthFormat);

        await FinancialSalesEntrySync.EnsureSalesEntries(db, user.Id, cancellationToken);
        await FinancialStockPurchaseSync.EnsureFinancialEntries(db, user.Id, cancellationToken);
        await FinancialFixedAccountSync.EnsureEntriesForMonth(db, user.Id, month, cancellationToken);

        var monthlyEntries = await db.FinancialEntries
            .Where(x => x.UserId == user.Id && x.Status != FinancialEntryStatus.Excluded && x.Date >= month.Start && x.Date <= month.End)
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.Id)
            .ToListAsync(cancellationToken);

        var accumulatedEntries = await db.FinancialEntries
            .Where(x => x.UserId == user.Id && x.Status != FinancialEntryStatus.Excluded && x.Date <= month.End)
            .ToListAsync(cancellationToken);

        var totalIncomes = monthlyEntries.Where(x => x.Type == FinancialEntryType.Income).Sum(x => x.Amount);
        var totalExpenses = monthlyEntries.Where(x => x.Type == FinancialEntryType.Expense).Sum(x => x.Amount);
        var pendingToReceive = monthlyEntries.Where(x => x.Type == FinancialEntryType.Income && x.Status == FinancialEntryStatus.Pending).Sum(x => x.Amount);
        var pendingToPay = monthlyEntries.Where(x => x.Type == FinancialEntryType.Expense && x.Status == FinancialEntryStatus.Pending).Sum(x => x.Amount);
        var fixedCostsInMonth = monthlyEntries.Where(x => x.IsFixed && x.Type == FinancialEntryType.Expense).Sum(x => x.Amount);

        var currentBalance = accumulatedEntries.Where(x => x.Type == FinancialEntryType.Income && x.Status == FinancialEntryStatus.Paid).Sum(x => x.Amount)
            - accumulatedEntries.Where(x => x.Type == FinancialEntryType.Expense && x.Status == FinancialEntryStatus.Paid).Sum(x => x.Amount);

        var forecastBalance = accumulatedEntries.Where(x => x.Type == FinancialEntryType.Income && x.Status != FinancialEntryStatus.Excluded).Sum(x => x.Amount)
            - accumulatedEntries.Where(x => x.Type == FinancialEntryType.Expense && x.Status != FinancialEntryStatus.Excluded).Sum(x => x.Amount);

        return ResultOf<FinancialOverviewModel>.Success(new FinancialOverviewModel
        {
            SelectedMonth = month.Value,
            TotalIncomes = totalIncomes,
            TotalExpenses = totalExpenses,
            PendingToReceive = pendingToReceive,
            PendingToPay = pendingToPay,
            CurrentBalance = currentBalance,
            ForecastBalance = forecastBalance,
            FixedCostsInMonth = fixedCostsInMonth,
            RecentEntries = monthlyEntries.Take(10).Select(x => x.ToModel()).ToList()
        });
    }
}
