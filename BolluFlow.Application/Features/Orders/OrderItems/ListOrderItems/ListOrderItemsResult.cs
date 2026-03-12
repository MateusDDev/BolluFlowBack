namespace BolluFlow.Application.Features.Orders.OrderItems.ListOrderItems;

public class ListOrderItemsResult
{
    public long Id { get; set; }
    public long OrderId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public ListOrderItemsResultProductModel Product { get; set; } = null!;
}

public class ListOrderItemsResultProductModel
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
}
