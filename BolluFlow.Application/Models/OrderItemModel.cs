namespace BolluFlow.Application.Models;

public class OrderItemModel
{
    public long Id { get; set; }
    public long OrderId { get; set; }
    public long ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public ProductRefModel Product { get; set; } = null!;
}

public class ProductRefModel
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
}
