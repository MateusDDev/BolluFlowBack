using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Features.Orders.GetOrderDetails;

public class GetOrderDetailsResult
{
    public long Id { get; set; }

    public string CustomerName { get; set; }

    public OrderStatus Status { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<GetOrderDetailsResultItem> Items { get; set; } = [];
}

public class GetOrderDetailsResultItem
{
    public long Id { get; set; }
    
    public int Quantity { get; set; }
    
    public decimal UnitPrice { get; set; }
    
    public decimal UnitCost { get; set; }
}