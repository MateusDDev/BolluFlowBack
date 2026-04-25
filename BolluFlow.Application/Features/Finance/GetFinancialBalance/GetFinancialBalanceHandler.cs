using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Finance.GetFinancialBalance;

public class GetFinancialBalanceHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<GetFinancialBalanceQuery, ResultOf<FinancialBalanceModel>>
{
    public async Task<ResultOf<FinancialBalanceModel>> Handle(GetFinancialBalanceQuery request, CancellationToken cancellationToken)
    {
        if (request.Month.HasValue ^ request.Year.HasValue)
            return ResultOf<FinancialBalanceModel>.BadRequest(new("FINANCIAL_PERIOD_INVALID", "month and year must be informed together"));

        if (request.Month is < 1 or > 12)
            return ResultOf<FinancialBalanceModel>.BadRequest(new("FINANCIAL_MONTH_INVALID", "month must be between 1 and 12"));

        if (request.Year is < 2000 or > 2100)
            return ResultOf<FinancialBalanceModel>.BadRequest(new("FINANCIAL_YEAR_INVALID", "year must be between 2000 and 2100"));

        var baseQuery = db.FinancialEntries.AsNoTracking().Where(x => x.UserId == user.Id);

        decimal monthIncome = 0;
        decimal monthExpenses = 0;
        decimal totalIncome;
        decimal totalExpenses;

        if (request.Month.HasValue && request.Year.HasValue)
        {
            var monthStart = new DateTime(request.Year.Value, request.Month.Value, 1, 0, 0, 0, DateTimeKind.Utc);
            var nextMonth = monthStart.AddMonths(1);

            monthIncome = await baseQuery
                .Where(x => x.Type == FinancialEntryType.Income && x.CreatedAt >= monthStart && x.CreatedAt < nextMonth)
                .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0;

            monthExpenses = await baseQuery
                .Where(x => x.Type == FinancialEntryType.Expense && x.CreatedAt >= monthStart && x.CreatedAt < nextMonth)
                .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0;

            totalIncome = await baseQuery
                .Where(x => x.Type == FinancialEntryType.Income && x.CreatedAt < nextMonth)
                .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0;

            totalExpenses = await baseQuery
                .Where(x => x.Type == FinancialEntryType.Expense && x.CreatedAt < nextMonth)
                .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0;
        }
        else
        {
            totalIncome = await baseQuery
                .Where(x => x.Type == FinancialEntryType.Income)
                .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0;

            totalExpenses = await baseQuery
                .Where(x => x.Type == FinancialEntryType.Expense)
                .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0;
        }

        var pendingOrdersTotal = await db.Orders
            .AsNoTracking()
            .Where(x => x.Customer.UserId == user.Id && x.Status != OrderStatus.Canceled && x.Status != OrderStatus.Delivered)
            .Select(x => new
            {
                Subtotal = x.Items.Sum(i => i.Quantity * i.UnitPrice),
                x.Discount,
                x.DiscountType,
                x.ShippingFee
            })
            .Select(x => x.Subtotal
                         - (x.Discount.HasValue && x.DiscountType.HasValue
                             ? (x.DiscountType == DiscountType.Percentage
                                 ? x.Subtotal * (x.Discount.Value / 100)
                                 : x.Discount.Value)
                             : 0)
                         + x.ShippingFee)
            .SumAsync(cancellationToken);

        var balance = totalIncome - totalExpenses;

        return ResultOf<FinancialBalanceModel>.Success(new FinancialBalanceModel
        {
            MonthIncome = monthIncome,
            MonthExpenses = monthExpenses,
            TotalIncome = totalIncome,
            TotalExpenses = totalExpenses,
            Balance = balance,
            PendingOrdersTotal = pendingOrdersTotal,
            ProjectedBalance = balance + pendingOrdersTotal
        });
    }
}
