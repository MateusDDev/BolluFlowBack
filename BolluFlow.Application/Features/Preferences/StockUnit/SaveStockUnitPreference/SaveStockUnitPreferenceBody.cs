namespace BolluFlow.Application.Features.Preferences.StockUnit.SaveStockUnitPreference;

public record SaveStockUnitPreferenceBody(string ItemType, long ItemId, long? ConversionId);
