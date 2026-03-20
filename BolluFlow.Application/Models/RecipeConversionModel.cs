namespace BolluFlow.Application.Models;

public class RecipeConversionModel
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal QuantityInBaseUnit { get; set; }
}
