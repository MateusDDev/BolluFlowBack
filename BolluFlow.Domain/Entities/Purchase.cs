using BolluFlow.Domain.Enums;

namespace BolluFlow.Domain.Entities;

public class Purchase
{
    private Purchase() { }

    public Guid Id { get; private set; }
    public PurchaseItemType ItemType { get; private set; }
    public long ItemId { get; private set; }
    public string ItemNameSnapshot { get; private set; } = null!;
    public string ConversionNameSnapshot { get; private set; } = null!;
    public decimal Quantity { get; private set; }
    public decimal QuantityInBaseUnit { get; private set; }
    public decimal TotalCost { get; private set; }
    public DateOnly PurchasedAt { get; private set; }
    public string? Notes { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public static Purchase Create(
        PurchaseItemType itemType,
        long itemId,
        string itemNameSnapshot,
        string conversionNameSnapshot,
        decimal quantity,
        decimal quantityInBaseUnit,
        decimal totalCost,
        DateOnly purchasedAt,
        string? notes)
    {
        var utcNow = DateTime.UtcNow;
        return new Purchase
        {
            Id = Guid.NewGuid(),
            ItemType = itemType,
            ItemId = itemId,
            ItemNameSnapshot = itemNameSnapshot,
            ConversionNameSnapshot = conversionNameSnapshot,
            Quantity = decimal.Round(quantity, 3, MidpointRounding.AwayFromZero),
            QuantityInBaseUnit = decimal.Round(quantityInBaseUnit, 3, MidpointRounding.AwayFromZero),
            TotalCost = decimal.Round(totalCost, 2, MidpointRounding.AwayFromZero),
            PurchasedAt = purchasedAt,
            Notes = notes,
            CreatedAt = utcNow,
            UpdatedAt = utcNow
        };
    }

    public void SoftDelete()
    {
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
