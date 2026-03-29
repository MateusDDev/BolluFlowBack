using BolluFlow.Domain.Enums;

namespace BolluFlow.Domain.Entities;

public class StockMovement
{
    private StockMovement() { }

    public long Id { get; private set; }
    public StockItemType ItemType { get; private set; }
    public long ItemId { get; private set; }
    public StockMovementType MovementType { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitCost { get; private set; }
    public decimal TotalValue { get; private set; }
    public string? Description { get; private set; }
    public string? ReferenceType { get; private set; }
    public long? ReferenceId { get; private set; }
    public string? EventKey { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public static StockMovement Create(
        StockItemType itemType,
        long itemId,
        StockMovementType movementType,
        decimal quantity,
        decimal unitCost,
        string? description,
        string? referenceType,
        long? referenceId,
        string? eventKey)
    {
        var now = DateTime.UtcNow;

        return new StockMovement
        {
            ItemType = itemType,
            ItemId = itemId,
            MovementType = movementType,
            Quantity = quantity,
            UnitCost = unitCost,
            TotalValue = decimal.Round(quantity * unitCost, 2, MidpointRounding.AwayFromZero),
            Description = description,
            ReferenceType = referenceType,
            ReferenceId = referenceId,
            EventKey = eventKey,
            CreatedAt = now,
            UpdatedAt = now
        };
    }
}
