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
    public FinancialEntrySource Source { get; private set; }
    public long? ReferenceId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static FinancialEntry CreateManual(
        long userId,
        string description,
        FinancialEntryType type,
        string category,
        decimal amount,
        DateOnly date,
        FinancialEntryStatus status,
        bool isFixed,
        string? notes)
    {
        return Create(
            userId,
            description,
            type,
            category,
            amount,
            date,
            status,
            isFixed,
            notes,
            FinancialEntrySource.Manual,
            null);
    }

    public static FinancialEntry CreateAutomatic(
        long userId,
        string description,
        FinancialEntryType type,
        string category,
        decimal amount,
        DateOnly date,
        FinancialEntryStatus status,
        bool isFixed,
        string? notes,
        FinancialEntrySource source,
        long referenceId)
    {
        if (source == FinancialEntrySource.Manual)
            throw new DomainException(FinancialErrors.InvalidAutomaticSource);

        return Create(userId, description, type, category, amount, date, status, isFixed, notes, source, referenceId);
    }

    private static FinancialEntry Create(
        long userId,
        string description,
        FinancialEntryType type,
        string category,
        decimal amount,
        DateOnly date,
        FinancialEntryStatus status,
        bool isFixed,
        string? notes,
        FinancialEntrySource source,
        long? referenceId)
    {
        Validate(description, amount, source, referenceId);

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
            Source = source,
            ReferenceId = referenceId,
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
        if (Source != FinancialEntrySource.Manual)
            throw new DomainException(FinancialErrors.AutomaticEntryCannotBeEdited);

        if (Type != type)
            throw new DomainException(FinancialErrors.FinancialEntryTypeImmutable);

        Validate(description, amount, Source, ReferenceId);

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

    private static void Validate(string description, decimal amount, FinancialEntrySource source, long? referenceId)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException(FinancialErrors.DescriptionRequired);

        if (amount <= 0)
            throw new DomainException(FinancialErrors.AmountMustBePositive);

        if (source != FinancialEntrySource.Manual && referenceId is null)
            throw new DomainException(FinancialErrors.ReferenceRequiredForAutomaticEntry);
    }
}
