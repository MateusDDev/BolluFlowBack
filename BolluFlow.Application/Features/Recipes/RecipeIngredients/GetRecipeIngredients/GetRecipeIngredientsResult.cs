namespace BolluFlow.Application.Features.Recipes.RecipeIngredients.GetRecipeIngredients;

public class GetRecipeIngredientsResult
{
    public long RecipeId { get; init; }
    public long IngredientId { get; init; }
    public string Name { get; init; }
    public string Unit { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal StockQuantity { get; init; }
    public decimal? MinimumStockQuantity { get; init; }
    public decimal QuantityUsed { get; init; }
}