using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Errors;

namespace BolluFlow.Domain.Entities;

public class IngredientConversion
{
    private IngredientConversion() { }

    public long Id { get; private set; }

    public long IngredientId { get; private set; }
    
    public Ingredient Ingredient { get; private set; } = null!;

    public string Name { get; private set; } = null!; 
    
    public decimal QuantityInBaseUnit { get; private set; }

    public static IngredientConversion Create(
        long ingredientId,
        string name,
        decimal quantityInBaseUnit)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(IngredientConversionErrors.NameRequired);

        if (quantityInBaseUnit <= 0)
            throw new DomainException(IngredientConversionErrors.QuantityInvalid);

        return new IngredientConversion
        {
            IngredientId = ingredientId,
            Name = name,
            QuantityInBaseUnit = quantityInBaseUnit
        };
    }
    
    public void Update(string name, decimal quantityInBaseUnit)
    {
        Name = name;
        QuantityInBaseUnit = quantityInBaseUnit;
    }
}