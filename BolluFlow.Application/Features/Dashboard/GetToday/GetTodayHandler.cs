using BolluFlow.Application.Abstractions;
using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Dashboard.GetToday;

public sealed class GetTodayHandler(ApplicationDbContext db, CurrentUser currentUser)
    : IRequestHandler<GetTodayQuery, ResultOf<GetTodayResult>>
{
    public async Task<ResultOf<GetTodayResult>> Handle(GetTodayQuery request, CancellationToken cancellationToken)
    {
        var todayStart = DateTime.SpecifyKind(request.DayStart, DateTimeKind.Utc);
        var tomorrowStart = todayStart.AddDays(1);
        var afterTomorrowStart = todayStart.AddDays(2);

        var openOrders = await db.Orders
            .AsNoTracking()
            .Include(x => x.Customer)
            .Include(x => x.Items)
            .Where(x => x.UserId == currentUser.Id
                        && x.Type == OrderType.CustomOrder
                        && x.Status != OrderStatus.Delivered
                        && x.Status != OrderStatus.Canceled)
            .ToListAsync(cancellationToken);

        List<TodayOrder> Select(Func<Order, bool> predicate) => openOrders
            .Where(predicate)
            .OrderBy(x => x.DeliveryDate)
            .ThenBy(x => x.Number)
            .Select(ToTodayOrder)
            .ToList();

        return ResultOf<GetTodayResult>.Success(new GetTodayResult(
            Select(x => x.DeliveryDate < todayStart),
            Select(x => x.DeliveryDate >= todayStart && x.DeliveryDate < tomorrowStart),
            Select(x => x.DeliveryDate >= tomorrowStart && x.DeliveryDate < afterTomorrowStart),
            Select(x => x.Status == OrderStatus.Ready && (x.DeliveryDate is null || x.DeliveryDate >= afterTomorrowStart))));
    }

    private static TodayOrder ToTodayOrder(Order order) => new(
        order.Id,
        order.Number,
        order.Customer?.Name,
        order.Status.ToString(),
        order.DeliveryDate,
        decimal.Round(order.CalculateTotal(), 2, MidpointRounding.AwayFromZero));
}
