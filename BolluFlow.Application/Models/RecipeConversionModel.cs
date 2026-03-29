namespace BolluFlow.Application.Models;

public class RecipeConversionModel
{
    public long Id { get; set; }

    public long RecipeId { get; set; }

    public required string Name { get; set; }

    public decimal QuantityInBaseUnit { get; set; }
}
