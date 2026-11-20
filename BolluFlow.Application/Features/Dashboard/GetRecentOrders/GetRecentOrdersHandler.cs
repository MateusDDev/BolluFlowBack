using BolluFlow.Application.Abstractions;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Dashboard.GetRecentOrders;

public sealed class GetRecentOrdersHandler(ApplicationDbContext db, CurrentUser currentUser)
    : IRequestHandler<GetRecentOrdersQuery, ResultOf<List<GetRecentOrdersResult>>>
{
    public async Task<ResultOf<List<GetRecentOrdersResult>>> Handle(GetRecentOrdersQuery request, CancellationToken cancellationToken)
    {
        var effectiveLimit = Math.Clamp(request.Limit ?? 5, 1, 20);

        var orders = await db.Orders
            .AsNoTracking()
            .Where(x => x.UserId == currentUser.Id)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new GetRecentOrdersResult(
                x.Id,
                x.Number,
                x.Customer != null ? x.Customer.Name : null,
                x.Status.ToString(),
                x.CreatedAt,
                x.Items.Sum(i => i.Quantity),
                decimal.Round(x.Items.Sum(i => i.UnitPrice * i.Quantity), 2)))
            .Take(effectiveLimit)
            .ToListAsync(cancellationToken);

        return ResultOf<List<GetRecentOrdersResult>>.Success(orders);
    }
}
