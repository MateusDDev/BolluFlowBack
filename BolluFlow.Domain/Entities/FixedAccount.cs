using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Enums;
using BolluFlow.Domain.Errors;

namespace BolluFlow.Domain.Entities;

public class FixedAccount
{
    protected FixedAccount() { }

    public long Id { get; private set; }
    public long UserId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public DateOnly StartMonth { get; private set; }
    public DateOnly? EndMonth { get; private set; }
    public FinancialEntryStatus DefaultStatus { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static FixedAccount Create(long userId, string description, string category, decimal amount, DateOnly startMonth, FinancialEntryStatus defaultStatus)
    {
        Validate(description, amount, startMonth, defaultStatus);

        return new FixedAccount
        {
            UserId = userId,
            Description = description.Trim(),
            Category = category.Trim(),
            Amount = amount,
            StartMonth = new DateOnly(startMonth.Year, startMonth.Month, 1),
            DefaultStatus = defaultStatus,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string description, string category, decimal amount, DateOnly startMonth, FinancialEntryStatus defaultStatus)
    {
        Validate(description, amount, startMonth, defaultStatus);

        Description = description.Trim();
        Category = category.Trim();
        Amount = amount;
        StartMonth = new DateOnly(startMonth.Year, startMonth.Month, 1);
        DefaultStatus = defaultStatus;
    }

    public void ApplyDeleteRange(DeletionMode mode, DateOnly? month)
    {
        switch (mode)
        {
            case DeletionMode.All:
                IsDeleted = true;
                EndMonth = StartMonth.AddMonths(-1);
                return;
            case DeletionMode.UntilMonth:
                if (month is null)
                    throw new DomainException(FinancialErrors.InvalidMonthFormat);
                StartMonth = new DateOnly(month.Value.Year, month.Value.Month, 1).AddMonths(1);
                return;
            case DeletionMode.FromMonth:
                if (month is null)
                    throw new DomainException(FinancialErrors.InvalidMonthFormat);
                EndMonth = new DateOnly(month.Value.Year, month.Value.Month, 1).AddMonths(-1);
                return;
        }
    }

    public bool AppliesTo(DateOnly monthStart)
    {
        if (IsDeleted)
            return false;

        var normalizedMonth = new DateOnly(monthStart.Year, monthStart.Month, 1);

        if (StartMonth > normalizedMonth)
            return false;

        return EndMonth is null || EndMonth.Value >= normalizedMonth;
    }

    private static void Validate(string description, decimal amount, DateOnly startMonth, FinancialEntryStatus defaultStatus)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException(FinancialErrors.FixedAccountDescriptionRequired);

        if (amount <= 0)
            throw new DomainException(FinancialErrors.FixedAccountAmountMustBePositive);

        if (defaultStatus == FinancialEntryStatus.Excluded)
            throw new DomainException(FinancialErrors.InvalidStatusTransition);

        _ = new DateOnly(startMonth.Year, startMonth.Month, 1);
    }
}
