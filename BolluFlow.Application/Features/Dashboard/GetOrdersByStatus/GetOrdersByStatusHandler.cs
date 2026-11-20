using BolluFlow.Application.Abstractions;
using BolluFlow.Domain.Enums;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Dashboard.GetOrdersByStatus;

public sealed class GetOrdersByStatusHandler(ApplicationDbContext db, CurrentUser currentUser)
    : IRequestHandler<GetOrdersByStatusQuery, ResultOf<List<GetOrdersByStatusResult>>>
{
    public async Task<ResultOf<List<GetOrdersByStatusResult>>> Handle(GetOrdersByStatusQuery request, CancellationToken cancellationToken)
    {
        var grouped = await db.Orders
            .AsNoTracking()
            .Where(x => x.UserId == currentUser.Id)
            .GroupBy(x => x.Status)
            .Select(x => new { Status = x.Key, Total = x.Count() })
            .ToListAsync(cancellationToken);

        var map = grouped.ToDictionary(x => x.Status, x => x.Total);

        var response = Enum.GetValues<OrderStatus>()
            .Select(status => new GetOrdersByStatusResult(status.ToString(), map.GetValueOrDefault(status, 0)))
            .ToList();

        return ResultOf<List<GetOrdersByStatusResult>>.Success(response);
    }
}
