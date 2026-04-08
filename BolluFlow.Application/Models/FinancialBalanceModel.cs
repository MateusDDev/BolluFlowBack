namespace BolluFlow.Application.Models;

public class FinancialBalanceModel
{
    public decimal MonthIncome { get; set; }
    public decimal MonthExpenses { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal Balance { get; set; }
    public decimal PendingOrdersTotal { get; set; }
    public decimal ProjectedBalance { get; set; }
}
