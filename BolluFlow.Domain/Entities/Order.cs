using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Enums;
using BolluFlow.Domain.Errors;

namespace BolluFlow.Domain.Entities;

public class Order
{
    private Order() { }

    public long Id { get; private set; }

    public long CustomerId { get; private set; }

    public Customer Customer { get; private set; } = null!;

    public OrderStatus Status { get; set; }

    public string? Note { get; set; }
    
    public DateTime? DeliveryDate { get; set; }

    public DateTime CreatedAt { get; private set; }

    private readonly List<OrderItem> _items = [];
    public IReadOnlyCollection<OrderItem> Items => _items;

    public static Order Create(long customerId, string? note = null, DateTime? deliveryDate = null)
    {
        return new Order
        {
            CustomerId = customerId,
            Status = OrderStatus.Pending,
            Note = note,
            DeliveryDate = deliveryDate,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    public void Update(string? note)
    {
        Note = note;
    }

    public void AddItem(Recipe recipe, int quantity)
    {
        if (!recipe.IsActive)
            throw new DomainException(OrderErrors.RecipeInactive);

        var unitCost = recipe.CalculateUnitCost();

        foreach (var ingredient in recipe.RecipeIngredients)
        {
            ingredient.Ingredient.ConsumeStock(ingredient.QuantityUsed * quantity);
        }

        _items.Add(new OrderItem
        {
            RecipeId = recipe.Id,
            Quantity = quantity,
            UnitCost = unitCost,
            UnitPrice = recipe.FinalPrice
        });
    }
}