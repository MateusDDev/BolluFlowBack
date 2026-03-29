namespace BolluFlow.Application.Models;

public class RecipeProductionModel
{
    public long Id { get; set; }

    public long RecipeId { get; set; }

    public decimal Quantity { get; set; }

    public string? Notes { get; set; }

    public List<RecipeProductionDebitModel> Debits { get; set; } = [];

    public DateTime CreatedAt { get; set; }
}

public class RecipeProductionDebitModel
{
    public long IngredientId { get; set; }

    public required string IngredientName { get; set; }

    public decimal QuantityDebited { get; set; }

    public required string Unit { get; set; }
}
