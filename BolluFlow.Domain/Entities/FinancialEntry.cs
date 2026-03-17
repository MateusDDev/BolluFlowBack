using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Enums;
using BolluFlow.Domain.Errors;

namespace BolluFlow.Domain.Entities;

public class FinancialEntry
{
    protected FinancialEntry() { }

    public long Id { get; private set; }
    public long UserId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public FinancialEntryType Type { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public DateOnly Date { get; private set; }
    public FinancialEntryStatus Status { get; private set; }
    public bool IsFixed { get; private set; }
    public string? Notes { get; private set; }
    public bool IsAutomaticFromSale { get; private set; }
    public long? SourceOrderId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static FinancialEntry Create(
        long userId,
        string description,
        FinancialEntryType type,
        string category,
        decimal amount,
        DateOnly date,
        FinancialEntryStatus status,
        bool isFixed,
        string? notes,
        bool isAutomaticFromSale = false,
        long? sourceOrderId = null)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException(FinancialErrors.DescriptionRequired);

        if (amount <= 0)
            throw new DomainException(FinancialErrors.AmountMustBePositive);

        return new FinancialEntry
        {
            UserId = userId,
            Description = description.Trim(),
            Type = type,
            Category = category.Trim(),
            Amount = amount,
            Date = date,
            Status = status,
            IsFixed = isFixed,
            Notes = notes,
            IsAutomaticFromSale = isAutomaticFromSale,
            SourceOrderId = sourceOrderId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string description,
        FinancialEntryType type,
        string category,
        decimal amount,
        DateOnly date,
        FinancialEntryStatus status,
        bool isFixed,
        string? notes)
    {
        if (Type != type)
            throw new DomainException(FinancialErrors.FinancialEntryTypeImmutable);

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException(FinancialErrors.DescriptionRequired);

        if (amount <= 0)
            throw new DomainException(FinancialErrors.AmountMustBePositive);

        Description = description.Trim();
        Category = category.Trim();
        Amount = amount;
        Date = date;
        Status = status;
        IsFixed = isFixed;
        Notes = notes;
    }

    public void UpdateStatus(FinancialEntryStatus status)
    {
        Status = status;
    }
}
