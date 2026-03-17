using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Models;

public class SaleFinancialEntryModel
{
    public string Id { get; init; } = string.Empty;
    public long OrderId { get; init; }
    public string Description { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public DateOnly Date { get; init; }
    public FinancialEntryStatus Status { get; init; }
}
