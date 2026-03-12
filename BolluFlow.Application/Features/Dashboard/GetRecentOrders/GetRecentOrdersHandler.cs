using BolluFlow.Application.Abstractions;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Dashboard.GetRecentOrders;

public class GetRecentOrdersHandler(ApplicationDbContext db, CurrentUser currentUser)
    : IRequestHandler<GetRecentOrdersQuery, ResultOf<List<GetRecentOrdersResult>>>
{
    public async Task<ResultOf<List<GetRecentOrdersResult>>> Handle(GetRecentOrdersQuery request, CancellationToken cancellationToken)
    {
        var effectiveLimit = Math.Clamp(request.Limit ?? 5, 1, 20);

        var result = await db.Orders
            .AsNoTracking()
            .Where(x => x.Customer.UserId == currentUser.Id)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new GetRecentOrdersResult(
                x.Id,
                x.Number,
                x.Customer.Name,
                x.Status.ToString(),
                x.CreatedAt,
                x.Items.Sum(i => i.Quantity),
                decimal.Round(x.Items.Sum(i => i.UnitPrice * i.Quantity), 2)))
            .Take(effectiveLimit)
            .ToListAsync(cancellationToken);

        return ResultOf<List<GetRecentOrdersResult>>.Success(result);
    }
}
