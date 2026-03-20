using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Features.Finance.Shared;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Finance.ListFixedCosts;

public class ListFixedCostsHandler(ApplicationDbContext db) : IRequestHandler<ListFixedCostsQuery, ResultOf<List<FixedCostDto>>>
{
    public async Task<ResultOf<List<FixedCostDto>>> Handle(ListFixedCostsQuery request, CancellationToken cancellationToken)
    {
        var items = await db.FixedCosts
            .Where(x => x.DeletedAt == null)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return ResultOf<List<FixedCostDto>>.Success(items.Select(x => x.ToDto()).ToList());
    }
}
