namespace BolluFlow.Domain.Entities;

public class OrderItem
{
    public long Id { get; set; }
    
    public long OrderId { get; set; }
    
    public Order? Order { get; set; }
    
    public long RecipeId { get; set; }
    
    public Recipe Recipe { get; set; }
    
    public int Quantity { get; set; }
    
    public decimal UnitPrice { get; set; }
    
    public decimal UnitCost { get; set; }
}