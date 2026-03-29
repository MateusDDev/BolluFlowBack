using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Features.Supplies.UpdateSupply;

public record UpdateSupplyCommand(long Id, UpdateSupplyCommandBody Body) : ICommand<ResultOf<SupplyModel>>;

public record UpdateSupplyCommandBody(string Name, BaseUnit Unit, decimal UnitCost, decimal StockQuantity, decimal? MinimumStockQuantity);
