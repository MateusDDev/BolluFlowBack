namespace BolluFlow.Application.Models;

public class FinancialOverviewModel
{
    public decimal CurrentBalance { get; init; }
    public decimal TotalIncomesPaid { get; init; }
    public decimal TotalExpensesPaid { get; init; }
    public decimal PendingToReceive { get; init; }
    public decimal PendingToPay { get; init; }
    public decimal ForecastBalance { get; init; }
    public decimal FixedCostsInMonth { get; init; }
}
