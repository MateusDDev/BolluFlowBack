namespace BolluFlow.Application.Models;

public class StockUnitPreferenceModel
{
    public string ItemType { get; set; } = null!;
    public long ItemId { get; set; }
    public long? ConversionId { get; set; }
}
