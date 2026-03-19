using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Enums;
using BolluFlow.Domain.Errors;

namespace BolluFlow.Domain.Entities;

public class InventoryPurchaseMovement
{
    protected InventoryPurchaseMovement() { }

    public long Id { get; private set; }
    public long UserId { get; private set; }
    public InventoryPurchaseItemType ItemType { get; private set; }
    public long ItemId { get; private set; }
    public string ItemName { get; private set; } = string.Empty;
    public decimal QuantityAdded { get; private set; }
    public decimal UnitCost { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateOnly Date { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    public static InventoryPurchaseMovement Create(
        long userId,
        InventoryPurchaseItemType itemType,
        long itemId,
        string itemName,
        decimal quantityAdded,
        decimal unitCost,
        DateOnly date,
        string category,
        string description)
    {
        if (quantityAdded <= 0)
            throw new DomainException(FinancialErrors.StockPurchaseQuantityMustBePositive);

        if (unitCost <= 0)
            throw new DomainException(FinancialErrors.StockPurchaseUnitCostMustBePositive);

        return new InventoryPurchaseMovement
        {
            UserId = userId,
            ItemType = itemType,
            ItemId = itemId,
            ItemName = itemName.Trim(),
            QuantityAdded = quantityAdded,
            UnitCost = unitCost,
            TotalAmount = decimal.Round(quantityAdded * unitCost, 2, MidpointRounding.AwayFromZero),
            Date = date,
            Category = category.Trim(),
            Description = description.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }
}
