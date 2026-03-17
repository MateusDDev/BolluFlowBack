using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Errors;

namespace BolluFlow.Domain.Entities;

public class FixedAccount
{
    protected FixedAccount() { }

    public long Id { get; private set; }
    public long UserId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public decimal DefaultAmount { get; private set; }
    public int DueDay { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static FixedAccount Create(long userId, string description, string category, decimal defaultAmount, int dueDay, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException(FinancialErrors.FixedAccountDescriptionRequired);

        if (defaultAmount <= 0)
            throw new DomainException(FinancialErrors.FixedAccountAmountMustBePositive);

        if (dueDay is < 1 or > 28)
            throw new DomainException(FinancialErrors.DueDayInvalid);

        return new FixedAccount
        {
            UserId = userId,
            Description = description.Trim(),
            Category = category.Trim(),
            DefaultAmount = defaultAmount,
            DueDay = dueDay,
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow
        };
    }
}
