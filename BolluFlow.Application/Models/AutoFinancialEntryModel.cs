using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Models;

public class AutoFinancialEntryModel
{
    public string Id { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public FinancialEntryType Type { get; init; }
    public string Category { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public DateOnly Date { get; init; }
    public FinancialEntryStatus Status { get; init; }
    public FinancialEntrySource Source { get; init; }
    public long ReferenceId { get; init; }
}
