namespace BolluFlow.Application.Features.Orders.OrderItems.ListOrderItems;

public class ListOrderItemsResult
{
    public long Id { get; set; }
    
    public long OrderId { get; set; }
    
    public int Quantity { get; set; }
    
    public decimal UnitPrice { get; set; }
    
    public decimal TotalPrice { get; set; }
    
    public ListOrderItemsResultRecipeModel Recipe { get; set; }
}

public class ListOrderItemsResultRecipeModel
{
    public long Id { get; set; }
    
    public string Name { get; set; }
    
    public decimal? DiscountPercentage { get; set; }
    
    public decimal FinalPrice { get; set; }
}