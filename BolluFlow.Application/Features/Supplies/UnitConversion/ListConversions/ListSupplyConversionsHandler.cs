using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Supplies.UnitConversion.ListConversions;

public class ListSupplyConversionsHandler(ApplicationDbContext db) : IRequestHandler<ListSupplyConversionsQuery, ResultOf<List<SupplyConversionModel>>>
{
    public async Task<ResultOf<List<SupplyConversionModel>>> Handle(ListSupplyConversionsQuery request, CancellationToken cancellationToken)
    {
        var supplyConversions = await db.SupplyConversions
            .Where(x => x.SupplyId == request.SupplyId)
            .ToListAsync(cancellationToken);

        return ResultOf<List<SupplyConversionModel>>.Success(supplyConversions.Select(SupplyConversionModel.From).ToList());
    }
}
