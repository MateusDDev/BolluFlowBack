using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Features.Ingredients.UpdateIngredient;

public record UpdateIngredientCommand(long Id, UpdateIngredientCommandBody Body) : ICommand<ResultOf<IngredientModel>>;

public record UpdateIngredientCommandBody(
    string Name,
    BaseUnit Unit,
    decimal UnitCost,
    decimal? MinimumStockQuantity);
