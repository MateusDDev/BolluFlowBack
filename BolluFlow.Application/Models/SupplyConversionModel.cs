using BolluFlow.Domain.Entities;

namespace BolluFlow.Application.Models;

public class SupplyConversionModel
{
    public long Id { get; private set; }

    public long SupplyId { get; private set; }

    public string Name { get; private set; } = null!;

    public decimal QuantityInBaseUnit { get; private set; }

    public static SupplyConversionModel From(SupplyConversion conversion)
    {
        return new SupplyConversionModel
        {
            Id = conversion.Id,
            SupplyId = conversion.SupplyId,
            Name = conversion.Name,
            QuantityInBaseUnit = conversion.QuantityInBaseUnit
        };
    }
}
