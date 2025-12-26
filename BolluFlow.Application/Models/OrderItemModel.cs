namespace BolluFlow.Application.Models;

public class OrderItemModel
{
    public long Id { get; set; }
    
    public long OrderId { get; set; }
    
    public long RecipeId { get; set; }
    
    public int Quantity { get; set; }
    
    public decimal UnitPrice { get; set; }
    
    public decimal UnitCost { get; set; }
}