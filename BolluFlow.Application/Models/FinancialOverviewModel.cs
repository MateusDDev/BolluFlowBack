namespace BolluFlow.Application.Models;

public class FinancialOverviewModel
{
    public string SelectedMonth { get; init; } = string.Empty;
    public decimal TotalIncomes { get; init; }
    public decimal TotalExpenses { get; init; }
    public decimal PendingToReceive { get; init; }
    public decimal PendingToPay { get; init; }
    public decimal CurrentBalance { get; init; }
    public decimal ForecastBalance { get; init; }
    public decimal FixedCostsInMonth { get; init; }
    public IReadOnlyList<FinancialEntryModel> RecentEntries { get; init; } = [];
}
