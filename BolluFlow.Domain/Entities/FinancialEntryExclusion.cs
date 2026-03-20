namespace BolluFlow.Domain.Entities;

public class FinancialEntryExclusion
{
    protected FinancialEntryExclusion() { }

    public long Id { get; private set; }
    public long FinancialEntryId { get; private set; }
    public FinancialEntry FinancialEntry { get; private set; } = null!;
    public long UserId { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public bool PreserveSource { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static FinancialEntryExclusion Create(long financialEntryId, long userId, string reason, bool preserveSource)
    {
        return new FinancialEntryExclusion
        {
            FinancialEntryId = financialEntryId,
            UserId = userId,
            Reason = reason.Trim(),
            PreserveSource = preserveSource,
            CreatedAt = DateTime.UtcNow
        };
    }
}
