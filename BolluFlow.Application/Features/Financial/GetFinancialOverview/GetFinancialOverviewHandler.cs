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

        var entries = await db.FinancialEntries
            .Where(x => x.UserId == user.Id && x.Date >= month.Start && x.Date <= month.End)
            .ToListAsync(cancellationToken);

        var totalIncomesPaid = entries.Where(x => x.Type == FinancialEntryType.Income && x.Status == FinancialEntryStatus.Paid).Sum(x => x.Amount);
        var totalExpensesPaid = entries.Where(x => x.Type == FinancialEntryType.Expense && x.Status == FinancialEntryStatus.Paid).Sum(x => x.Amount);
        var pendingToReceive = entries.Where(x => x.Type == FinancialEntryType.Income && x.Status == FinancialEntryStatus.Pending).Sum(x => x.Amount);
        var pendingToPay = entries.Where(x => x.Type == FinancialEntryType.Expense && x.Status == FinancialEntryStatus.Pending).Sum(x => x.Amount);
        var fixedCostsInMonth = entries
            .Where(x => x.IsFixed && x.Type == FinancialEntryType.Expense)
            .Sum(x => x.Amount);

        var currentBalance = totalIncomesPaid - totalExpensesPaid;
        var forecastBalance = currentBalance + pendingToReceive - pendingToPay;

        return ResultOf<FinancialOverviewModel>.Success(new FinancialOverviewModel
        {
            Month = month.Value,
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
