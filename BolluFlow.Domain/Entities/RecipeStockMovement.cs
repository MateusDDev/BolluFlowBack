namespace BolluFlow.Domain.Entities;

public class RecipeStockMovement
{
    private RecipeStockMovement() { }

    public long Id { get; private set; }
    public long RecipeId { get; private set; }
    public Recipe Recipe { get; private set; } = null!;
    public long? ConversionId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal QuantityInBaseUnit { get; private set; }
    public DateOnly ProductionDate { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static RecipeStockMovement Create(long recipeId, long? conversionId, decimal quantity, decimal quantityInBaseUnit, DateOnly productionDate)
    {
        return new RecipeStockMovement
        {
            RecipeId = recipeId,
            ConversionId = conversionId,
            Quantity = quantity,
            QuantityInBaseUnit = quantityInBaseUnit,
            ProductionDate = productionDate,
            CreatedAt = DateTime.UtcNow
        };
    }
}
