using System.Text.Json;
using BolluFlow.Domain.Enums;

namespace BolluFlow.Domain.Entities;

public class FinancialTransaction
{
    private FinancialTransaction() { }

    public Guid Id { get; private set; }
    public FinancialTransactionType Type { get; private set; }
    public FinancialTransactionSourceType SourceType { get; private set; }
    public string Description { get; private set; } = null!;
    public decimal Amount { get; private set; }
    public DateOnly EventDate { get; private set; }
    public string Month { get; private set; } = null!;
    public string? Notes { get; private set; }
    public FinancialSourceEntityType SourceEntityType { get; private set; }
    public Guid? SourceEntityId { get; private set; }
    public bool IsAutomatic { get; private set; }
    public string? Metadata { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public static FinancialTransaction Create(
        FinancialTransactionType type,
        FinancialTransactionSourceType sourceType,
        string description,
        decimal amount,
        DateOnly eventDate,
        FinancialSourceEntityType sourceEntityType,
        Guid? sourceEntityId,
        bool isAutomatic,
        string? notes,
        string? metadata)
    {
        var utcNow = DateTime.UtcNow;
        return new FinancialTransaction
        {
            Id = Guid.NewGuid(),
            Type = type,
            SourceType = sourceType,
            Description = description,
            Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero),
            EventDate = eventDate,
            Month = $"{eventDate:yyyy-MM}",
            SourceEntityType = sourceEntityType,
            SourceEntityId = sourceEntityId,
            IsAutomatic = isAutomatic,
            Notes = notes,
            Metadata = metadata,
            CreatedAt = utcNow,
            UpdatedAt = utcNow
        };
    }

    public void SoftDelete()
    {
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public JsonDocument? ReadMetadata()
        => string.IsNullOrWhiteSpace(Metadata) ? null : JsonDocument.Parse(Metadata);
}
