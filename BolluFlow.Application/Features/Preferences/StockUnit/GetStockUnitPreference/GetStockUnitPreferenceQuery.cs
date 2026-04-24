using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Preferences.StockUnit.GetStockUnitPreference;

public record GetStockUnitPreferenceQuery(string ItemType, long ItemId) : IQuery<ResultOf<StockUnitPreferenceModel>>;
