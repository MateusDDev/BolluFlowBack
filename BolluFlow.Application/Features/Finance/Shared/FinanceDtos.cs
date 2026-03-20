using System.Text.Json.Nodes;

namespace BolluFlow.Application.Features.Finance.Shared;

public class FinancialTransactionDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = null!;
    public string SourceType { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Amount { get; set; }
    public string EventDate { get; set; } = null!;
    public string Month { get; set; } = null!;
    public string SourceEntityType { get; set; } = null!;
    public Guid? SourceEntityId { get; set; }
    public bool IsAutomatic { get; set; }
    public string? Notes { get; set; }
    public JsonNode? Metadata { get; set; }
}

public class FixedCostDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Amount { get; set; }
    public string StartMonth { get; set; } = null!;
    public string? EndMonth { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class FinanceOverviewDto
{
    public string Month { get; set; } = null!;
    public decimal IncomeTotal { get; set; }
    public decimal ExpenseTotal { get; set; }
    public decimal MonthBalance { get; set; }
    public decimal ProjectedBalance { get; set; }
    public List<FinancialTransactionDto> RecentTransactions { get; set; } = [];
}
