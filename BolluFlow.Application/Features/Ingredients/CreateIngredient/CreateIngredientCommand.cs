using BolluFlow.Application.Abstractions;
using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Features.Ingredients.CreateIngredient;

public record CreateIngredientCommand( 
    string Name,
    IngredientBaseUnit Unit,
    decimal UnitPrice,
    decimal StockQuantity,
    decimal? MinimumStockQuantity) : ICommand<ResultOf<CreateIngredientResult>>;