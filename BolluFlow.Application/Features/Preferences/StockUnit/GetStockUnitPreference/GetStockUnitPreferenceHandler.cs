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
        if (!IsValidItemType(request.ItemType))
            return ResultOf<StockUnitPreferenceModel>.BadRequest(new("STOCK_UNIT_PREFERENCE_ITEM_TYPE_INVALID", "itemType must be ingredient or supply"));

        if (request.ItemId <= 0)
            return ResultOf<StockUnitPreferenceModel>.BadRequest(new("STOCK_UNIT_PREFERENCE_ITEM_ID_INVALID", "itemId must be greater than zero"));

        var normalizedItemType = request.ItemType.Trim().ToLowerInvariant();

        var preference = await db.UserStockUnitPreferences
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == user.Id && x.ItemType == normalizedItemType && x.ItemId == request.ItemId, cancellationToken);

        return ResultOf<StockUnitPreferenceModel>.Success(new StockUnitPreferenceModel
        {
            ItemType = normalizedItemType,
            ItemId = request.ItemId,
            ConversionId = preference?.ConversionId
        });
    }

    private static bool IsValidItemType(string itemType)
    {
        if (string.IsNullOrWhiteSpace(itemType))
            return false;

        var normalized = itemType.Trim().ToLowerInvariant();
        return normalized is "ingredient" or "supply";
    }
}
