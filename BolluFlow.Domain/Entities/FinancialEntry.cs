using BolluFlow.Domain.Enums;

namespace BolluFlow.Domain.Entities;

public class FinancialEntry
{
    private FinancialEntry() { }

    public long Id { get; private set; }
    public FinancialEntryType Type { get; private set; }
    public decimal Amount { get; private set; }
    public string Description { get; private set; } = null!;
    public FinancialOriginType Origin { get; private set; }
    public long? OriginId { get; private set; }
    public string? EventKey { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public static FinancialEntry Create(
        FinancialEntryType type,
        decimal amount,
        string description,
        FinancialOriginType origin,
        long? originId,
        string? eventKey)
    {
        var now = DateTime.UtcNow;

        return new FinancialEntry
        {
            Type = type,
            Amount = amount,
            Description = description,
            Origin = origin,
            OriginId = originId,
            EventKey = eventKey,
            CreatedAt = now,
            UpdatedAt = now
        };
    }
}
