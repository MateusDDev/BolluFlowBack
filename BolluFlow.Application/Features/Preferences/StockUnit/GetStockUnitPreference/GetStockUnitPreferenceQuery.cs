using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Features.Preferences.StockUnit.GetStockUnitPreference;

public record GetStockUnitPreferenceQuery(StockItemType ItemType, long ItemId) : IQuery<ResultOf<StockUnitPreferenceModel>>;
