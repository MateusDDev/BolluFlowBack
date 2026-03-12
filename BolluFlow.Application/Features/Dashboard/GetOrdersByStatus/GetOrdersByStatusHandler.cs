using BolluFlow.Application.Abstractions;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Dashboard.GetOrdersByStatus;

public class GetOrdersByStatusHandler(ApplicationDbContext db, CurrentUser currentUser)
    : IRequestHandler<GetOrdersByStatusQuery, ResultOf<List<GetOrdersByStatusResult>>>
{
    public async Task<ResultOf<List<GetOrdersByStatusResult>>> Handle(GetOrdersByStatusQuery request, CancellationToken cancellationToken)
    {
        var grouped = await db.Orders
            .AsNoTracking()
            .Where(x => x.Customer.UserId == currentUser.Id)
            .GroupBy(x => x.Status)
            .Select(x => new { x.Key, Total = x.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Total, cancellationToken);

        var orderedStatuses = new[]
        {
            OrderStatus.Pending,
            OrderStatus.InProduction,
            OrderStatus.Ready,
            OrderStatus.Delivered,
            OrderStatus.Canceled
        };

        var result = orderedStatuses
            .Select(status => new GetOrdersByStatusResult(status.ToString(), grouped.GetValueOrDefault(status, 0)))
            .ToList();

        return ResultOf<List<GetOrdersByStatusResult>>.Success(result);
    }
}
