using BolluFlow.Application.Abstractions;
using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Features.Ingredients.CreateIngredient;

public record CreateIngredientCommand( 
    string Name,
    BaseUnit Unit,
    decimal UnitCost,
    decimal StockQuantity,
    decimal? MinimumStockQuantity) : ICommand<ResultOf<CreateIngredientResult>>;