using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Models;

public class StockUnitPreferenceModel
{
    public StockItemType ItemType { get; set; }
    public long ItemId { get; set; }
    public long? ConversionId { get; set; }
}
