namespace BolluFlow.Application.Models;

public class FixedAccountModel
{
    public long Id { get; init; }
    public string Description { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public decimal DefaultAmount { get; init; }
    public int DueDay { get; init; }
    public bool IsActive { get; init; }
}
