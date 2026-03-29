namespace BolluFlow.Application.Models;

public class FinancialEntryModel
{
    public long Id { get; set; }
    public string Type { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Description { get; set; } = null!;
    public string Origin { get; set; } = null!;
    public long? OriginId { get; set; }
    public DateTime CreatedAt { get; set; }
}
