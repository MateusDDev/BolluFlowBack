using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Features.Preferences.StockUnit.SaveStockUnitPreference;

public record SaveStockUnitPreferenceBody(StockItemType ItemType, long ItemId, long? ConversionId);
