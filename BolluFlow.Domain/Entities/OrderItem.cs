using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Errors;

namespace BolluFlow.Domain.Entities;

public class OrderItem
{
    protected OrderItem() { }
    
    public long Id { get; set; }
    
    public long OrderId { get; set; }

    public Order Order { get; set; } = null!;
    
    public long RecipeId { get; set; }

    public Recipe Recipe { get; set; } = null!;
    
    public int Quantity { get; private set; }
    
    public decimal UnitPrice { get; private set; }
    
    public decimal UnitCost { get; private set; }

    private void UpdateQuantity(int quantity)
    {
        if (quantity < 0)
            throw new DomainException(OrderItemErrors.InvalidQuantity);
        
        Quantity = quantity;
    }

    public static OrderItem Create(long recipeId, int quantity, decimal unitPrice, decimal unitCost)
    {
        return new OrderItem
        {
            RecipeId = recipeId,
            Quantity = quantity,
            UnitPrice = unitPrice,
            UnitCost = unitCost
        };
    }
    
    public void Update(int quantity, decimal unitPrice, decimal unitCost)
    {
        UpdateQuantity(quantity);
        UnitPrice = unitPrice;
        UnitCost = unitCost;
    }
}