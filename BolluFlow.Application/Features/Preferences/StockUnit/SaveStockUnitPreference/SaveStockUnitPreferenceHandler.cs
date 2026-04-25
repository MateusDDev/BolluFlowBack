using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;
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

        if (body.ItemId <= 0)
            return ResultOf<StockUnitPreferenceModel>.BadRequest(new("STOCK_UNIT_PREFERENCE_ITEM_ID_INVALID", "itemId must be greater than zero"));

        var itemExists = body.ItemType == StockItemType.Ingredient
            ? await db.Ingredients.AnyAsync(x => x.Id == body.ItemId && x.UserId == user.Id, cancellationToken)
            : await db.Supplies.AnyAsync(x => x.Id == body.ItemId && x.UserId == user.Id, cancellationToken);

        if (!itemExists)
            return ResultOf<StockUnitPreferenceModel>.NotFound(new("STOCK_UNIT_PREFERENCE_ITEM_NOT_FOUND", "Item not found"));

        if (body.ConversionId.HasValue)
        {
            var conversionExists = body.ItemType == StockItemType.Ingredient
                ? await db.IngredientConversions.AnyAsync(x => x.Id == body.ConversionId.Value && x.IngredientId == body.ItemId, cancellationToken)
                : await db.SupplyConversions.AnyAsync(x => x.Id == body.ConversionId.Value && x.SupplyId == body.ItemId, cancellationToken);

            if (!conversionExists)
                return ResultOf<StockUnitPreferenceModel>.NotFound(new("STOCK_UNIT_PREFERENCE_CONVERSION_NOT_FOUND", "Conversion not found for this item"));
        }

        var preference = await db.UserStockUnitPreferences
            .FirstOrDefaultAsync(x => x.UserId == user.Id && x.ItemType == body.ItemType && x.ItemId == body.ItemId, cancellationToken);

        if (preference is null)
        {
            preference = UserStockUnitPreference.Create(user.Id, body.ItemType, body.ItemId, body.ConversionId);
            db.UserStockUnitPreferences.Add(preference);
        }
        else
        {
            preference.UpdateConversion(body.ConversionId);
        }

        await db.SaveChangesAsync(cancellationToken);

        return ResultOf<StockUnitPreferenceModel>.Success(new StockUnitPreferenceModel
        {
            ItemType = body.ItemType,
            ItemId = body.ItemId,
            ConversionId = preference.ConversionId
        });
    }
}
