using BolluFlow.Domain.Enums;

namespace BolluFlow.Domain.Entities;

public class UserStockUnitPreference
{
    private UserStockUnitPreference() { }

    public long Id { get; private set; }
    public long UserId { get; private set; }
    public StockItemType ItemType { get; private set; }
    public long ItemId { get; private set; }
    public long? ConversionId { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public static UserStockUnitPreference Create(long userId, StockItemType itemType, long itemId, long? conversionId)
    {
        return new UserStockUnitPreference
        {
            UserId = userId,
            ItemType = itemType,
            ItemId = itemId,
            ConversionId = conversionId,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void UpdateConversion(long? conversionId)
    {
        ConversionId = conversionId;
        UpdatedAt = DateTime.UtcNow;
    }
}
