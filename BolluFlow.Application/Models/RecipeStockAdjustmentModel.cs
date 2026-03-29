namespace BolluFlow.Application.Models;

public class RecipeStockAdjustmentModel
{
    public long RecipeId { get; set; }

    public decimal Quantity { get; set; }

    public string Direction { get; set; } = null!;

    public string Reason { get; set; } = null!;

    public decimal StockQuantity { get; set; }

    public DateTime CreatedAt { get; set; }
}
