using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Entities;
using BolluFlow.Infrastructure.Auth;
using BolluFlow.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BolluFlow.Application.Features.Preferences.StockUnit.SaveStockUnitPreference;

public class SaveStockUnitPreferenceHandler(ApplicationDbContext db, CurrentUser user)
    : IRequestHandler<SaveStockUnitPreferenceCommand, ResultOf<StockUnitPreferenceModel>>
{
    public async Task<ResultOf<StockUnitPreferenceModel>> Handle(SaveStockUnitPreferenceCommand request, CancellationToken cancellationToken)
    {
        var body = request.Body;

        if (!IsValidItemType(body.ItemType))
            return ResultOf<StockUnitPreferenceModel>.BadRequest(new("STOCK_UNIT_PREFERENCE_ITEM_TYPE_INVALID", "itemType must be ingredient or supply"));

        if (body.ItemId <= 0)
            return ResultOf<StockUnitPreferenceModel>.BadRequest(new("STOCK_UNIT_PREFERENCE_ITEM_ID_INVALID", "itemId must be greater than zero"));

        var normalizedItemType = body.ItemType.Trim().ToLowerInvariant();

        var itemExists = normalizedItemType == "ingredient"
            ? await db.Ingredients.AnyAsync(x => x.Id == body.ItemId && x.UserId == user.Id, cancellationToken)
            : await db.Supplies.AnyAsync(x => x.Id == body.ItemId && x.UserId == user.Id, cancellationToken);

        if (!itemExists)
            return ResultOf<StockUnitPreferenceModel>.NotFound(new("STOCK_UNIT_PREFERENCE_ITEM_NOT_FOUND", "Item not found"));

        if (body.ConversionId.HasValue)
        {
            var conversionExists = normalizedItemType == "ingredient"
                ? await db.IngredientConversions.AnyAsync(x => x.Id == body.ConversionId.Value && x.IngredientId == body.ItemId, cancellationToken)
                : await db.SupplyConversions.AnyAsync(x => x.Id == body.ConversionId.Value && x.SupplyId == body.ItemId, cancellationToken);

            if (!conversionExists)
                return ResultOf<StockUnitPreferenceModel>.NotFound(new("STOCK_UNIT_PREFERENCE_CONVERSION_NOT_FOUND", "Conversion not found for this item"));
        }

        var preference = await db.UserStockUnitPreferences
            .FirstOrDefaultAsync(x => x.UserId == user.Id && x.ItemType == normalizedItemType && x.ItemId == body.ItemId, cancellationToken);

        if (preference is null)
        {
            preference = UserStockUnitPreference.Create(user.Id, normalizedItemType, body.ItemId, body.ConversionId);
            db.UserStockUnitPreferences.Add(preference);
        }
        else
        {
            preference.UpdateConversion(body.ConversionId);
        }

        await db.SaveChangesAsync(cancellationToken);

        return ResultOf<StockUnitPreferenceModel>.Success(new StockUnitPreferenceModel
        {
            ItemType = normalizedItemType,
            ItemId = body.ItemId,
            ConversionId = preference.ConversionId
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
