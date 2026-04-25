using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Preferences.StockUnit.SaveStockUnitPreference;

public record SaveStockUnitPreferenceCommand(SaveStockUnitPreferenceBody Body) : ICommand<ResultOf<StockUnitPreferenceModel>>;
