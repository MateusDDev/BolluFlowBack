using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Ingredients.CreateIngredient;

public record CreateIngredientCommand( 
    string Name,
    string Unit,
    decimal UnitPrice,
    decimal StockQuantity,
    decimal? MinimumStockQuantity) : ICommand<ResultOf<CreateIngredientResult>>;