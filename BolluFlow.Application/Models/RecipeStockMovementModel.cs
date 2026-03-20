namespace BolluFlow.Application.Models;

public class RecipeStockMovementModel
{
    public long Id { get; set; }
    public long? ConversionId { get; set; }
    public decimal Quantity { get; set; }
    public decimal QuantityInBaseUnit { get; set; }
    public DateOnly ProductionDate { get; set; }
}
