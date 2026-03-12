using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Errors;

namespace BolluFlow.Domain.Entities;

public class SupplyConversion
{
    private SupplyConversion() { }

    public long Id { get; private set; }

    public long SupplyId { get; private set; }

    public Supply Supply { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public decimal QuantityInBaseUnit { get; private set; }

    public static SupplyConversion Create(long supplyId, string name, decimal quantityInBaseUnit)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(IngredientConversionErrors.NameRequired);

        if (quantityInBaseUnit <= 0)
            throw new DomainException(IngredientConversionErrors.QuantityInvalid);

        return new SupplyConversion
        {
            SupplyId = supplyId,
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
