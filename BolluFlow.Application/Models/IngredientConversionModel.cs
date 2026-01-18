namespace BolluFlow.Application.Models;

public class IngredientConversionModel
{
    public long Id { get; private set; }

    public long IngredientId { get; private set; }

    public string Name { get; private set; } = null!; 
    
    public decimal QuantityInBaseUnit { get; private set; }
}