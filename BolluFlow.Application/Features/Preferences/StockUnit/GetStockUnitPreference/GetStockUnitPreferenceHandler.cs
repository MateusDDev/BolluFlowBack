using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Preferences.StockUnit.GetStockUnitPreference;

public class GetStockUnitPreferenceHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<GetStockUnitPreferenceQuery, ResultOf<StockUnitPreferenceModel>>
{
    public async Task<ResultOf<StockUnitPreferenceModel>> Handle(GetStockUnitPreferenceQuery request, CancellationToken cancellationToken)
    {
        if (request.ItemId <= 0)
            return ResultOf<StockUnitPreferenceModel>.BadRequest(new("STOCK_UNIT_PREFERENCE_ITEM_ID_INVALID", "itemId must be greater than zero"));

        var preference = await db.UserStockUnitPreferences
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == user.Id && x.ItemType == request.ItemType && x.ItemId == request.ItemId, cancellationToken);

        return ResultOf<StockUnitPreferenceModel>.Success(new StockUnitPreferenceModel
        {
            ItemType = request.ItemType,
            ItemId = request.ItemId,
            ConversionId = preference?.ConversionId
        });
    }
}
