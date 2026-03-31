namespace BolluFlow.Application.Models;

public class ProductProductionModel
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public decimal Quantity { get; set; }
    public string? Notes { get; set; }
    public List<ProductProductionDebitModel> Debits { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}

public class ProductProductionDebitModel
{
    public string ComponentType { get; set; } = null!;
    public long ComponentId { get; set; }
    public string ComponentName { get; set; } = null!;
    public decimal QuantityDebited { get; set; }
}
