namespace BolluFlow.Application.Features.Dashboard.GetLowStockIngredients;

public record GetLowStockIngredientsResult(
    long IngredientId,
    string Name,
    decimal StockQuantity,
    decimal MinimumStockQuantity);