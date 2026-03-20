using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Models;

public class FixedAccountModel
{
    public long Id { get; init; }
    public string Description { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string StartMonth { get; init; } = string.Empty;
    public string? EndMonth { get; init; }
    public FinancialEntryStatus DefaultStatus { get; init; }
    public bool IsDeleted { get; init; }
}
