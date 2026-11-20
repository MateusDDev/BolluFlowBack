using BolluFlow.Application.Abstractions;
using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Dashboard.GetSalesSummary;

public sealed class GetSalesSummaryHandler(ApplicationDbContext db, CurrentUser currentUser)
    : IRequestHandler<GetSalesSummaryQuery, ResultOf<GetSalesSummaryResult>>
{
    private const int MonthsInHistory = 12;
    private const int TopProductsLimit = 5;

    public async Task<ResultOf<GetSalesSummaryResult>> Handle(GetSalesSummaryQuery request, CancellationToken cancellationToken)
    {
        var from = DateTime.SpecifyKind(request.From, DateTimeKind.Utc);
        var to = DateTime.SpecifyKind(request.To, DateTimeKind.Utc);

        if (to <= from)
            return ResultOf<GetSalesSummaryResult>.BadRequest(new DomainError("DASHBOARD_PERIOD_INVALID", "O fim do período precisa ser depois do início"));

        var offset = TimeSpan.FromMinutes(request.UtcOffsetMinutes);
        var previousFrom = request.PreviousFrom is { } explicitPreviousFrom && explicitPreviousFrom < from
            ? DateTime.SpecifyKind(explicitPreviousFrom, DateTimeKind.Utc)
            : from - (to - from);

        var localTo = to - offset - TimeSpan.FromTicks(1);
        var historyStartLocal = new DateTime(localTo.Year, localTo.Month, 1).AddMonths(-(MonthsInHistory - 1));
        var historyFrom = DateTime.SpecifyKind(historyStartLocal + offset, DateTimeKind.Utc);
        var loadFrom = historyFrom < previousFrom ? historyFrom : previousFrom;

        var orders = await db.Orders
            .AsNoTracking()
            .Include(x => x.Items)
            .ThenInclude(x => x.Product)
            .Where(x => x.UserId == currentUser.Id
                        && x.Status == OrderStatus.Delivered
                        && x.DeliveredAt >= loadFrom
                        && x.DeliveredAt < to)
            .ToListAsync(cancellationToken);

        var current = orders.Where(x => x.DeliveredAt >= from).ToList();
        var previous = orders.Where(x => x.DeliveredAt >= previousFrom && x.DeliveredAt < from).ToList();

        var monthly = Enumerable.Range(0, MonthsInHistory)
            .Select(i => historyStartLocal.AddMonths(i))
            .Select(month =>
            {
                var inMonth = orders
                    .Where(x => x.DeliveredAt >= historyFrom)
                    .Where(x =>
                    {
                        var local = x.DeliveredAt!.Value - offset;
                        return local.Year == month.Year && local.Month == month.Month;
                    })
                    .ToList();

                return new MonthlySales(month.Year, month.Month, Round(inMonth.Sum(x => x.CalculateTotal())), inMonth.Count);
            })
            .ToList();

        var topProducts = current
            .SelectMany(x => x.Items)
            .GroupBy(x => new { x.ProductId, x.Product.Name })
            .Select(x => new TopProduct(x.Key.ProductId, x.Key.Name, x.Sum(i => i.Quantity), Round(x.Sum(i => i.Quantity * i.UnitPrice))))
            .OrderByDescending(x => x.Quantity)
            .ThenByDescending(x => x.Revenue)
            .Take(TopProductsLimit)
            .ToList();

        var hasCostData = current.SelectMany(x => x.Items).Any(x => x.UnitCost > 0);

        return ResultOf<GetSalesSummaryResult>.Success(new GetSalesSummaryResult(
            Summarize(current),
            Summarize(previous),
            monthly,
            topProducts,
            hasCostData));
    }

    private static SalesPeriodMetrics Summarize(List<Order> orders)
    {
        var revenue = orders.Sum(x => x.CalculateTotal());
        var netSales = orders.Sum(x => x.CalculateSubtotal() - x.CalculateDiscount(x.CalculateSubtotal()));
        var cost = orders.SelectMany(x => x.Items).Sum(x => x.UnitCost * x.Quantity);
        var grossProfit = netSales - cost;

        return new SalesPeriodMetrics(
            Round(revenue),
            orders.Count,
            orders.Count == 0 ? 0 : Round(revenue / orders.Count),
            Round(grossProfit),
            netSales > 0 ? Math.Round(grossProfit / netSales * 100, 1) : null);
    }

    private static decimal Round(decimal value) => decimal.Round(value, 2, MidpointRounding.AwayFromZero);
}
