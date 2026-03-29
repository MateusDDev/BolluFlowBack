using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Errors;

namespace BolluFlow.Domain.Entities;

public class RecipeConversion
{
    private RecipeConversion() { }

    public long Id { get; private set; }

    public long RecipeId { get; private set; }

    public Recipe Recipe { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public decimal QuantityInBaseUnit { get; private set; }

    public static RecipeConversion Create(long recipeId, string name, decimal quantityInBaseUnit)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(IngredientConversionErrors.NameRequired);

        if (quantityInBaseUnit <= 0)
            throw new DomainException(IngredientConversionErrors.QuantityInvalid);

        return new RecipeConversion
        {
            RecipeId = recipeId,
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
