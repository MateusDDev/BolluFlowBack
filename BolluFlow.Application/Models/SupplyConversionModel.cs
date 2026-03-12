namespace BolluFlow.Application.Models;

public class SupplyConversionModel
{
    public long Id { get; private set; }

    public long SupplyId { get; private set; }

    public string Name { get; private set; } = null!;

    public decimal QuantityInBaseUnit { get; private set; }
}
