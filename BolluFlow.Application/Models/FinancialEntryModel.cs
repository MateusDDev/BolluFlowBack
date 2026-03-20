using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Models;

public class FinancialEntryModel
{
    public long Id { get; init; }
    public string Description { get; init; } = string.Empty;
    public FinancialEntryType Type { get; init; }
    public string Category { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public DateOnly Date { get; init; }
    public FinancialEntryStatus Status { get; init; }
    public bool IsFixed { get; init; }
    public string? Notes { get; init; }
    public FinancialEntrySource Source { get; init; }
    public FinancialReferenceType ReferenceType { get; init; }
    public long? ReferenceId { get; init; }
}
