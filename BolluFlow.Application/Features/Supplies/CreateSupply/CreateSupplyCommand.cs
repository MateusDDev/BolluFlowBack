using BolluFlow.Application.Abstractions;
using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Features.Supplies.CreateSupply;

public record CreateSupplyCommand(string Name, IngredientBaseUnit Unit, decimal UnitCost, decimal StockQuantity, decimal? MinimumStockQuantity)
    : ICommand<ResultOf<CreateSupplyResult>>;

public record CreateSupplyResult(long Id);
