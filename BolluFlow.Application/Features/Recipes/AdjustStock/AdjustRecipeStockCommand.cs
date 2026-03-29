using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Recipes.AdjustStock;

public record AdjustRecipeStockCommand(long RecipeId, AdjustRecipeStockCommandBody Body) : ICommand<ResultOf<RecipeStockAdjustmentModel>>;

public record AdjustRecipeStockCommandBody(decimal Quantity, string Direction, string Reason);
