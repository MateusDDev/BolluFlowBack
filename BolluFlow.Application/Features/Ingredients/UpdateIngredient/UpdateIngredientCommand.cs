using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Ingredients.UpdateIngredient;

public record UpdateIngredientCommand(long Id, UpdateIngredientCommandBody Body) : ICommand<ResultOf<IngredientModel>>;

public record UpdateIngredientCommandBody(
    string Name,
    string Unit, 
    decimal UnitPrice, 
    decimal StockQuantity, 
    decimal? MinimumStockQuantity);