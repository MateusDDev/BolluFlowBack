using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Enums;
using BolluFlow.Domain.Errors;

namespace BolluFlow.Domain.Entities;

public class Order
{
    private Order() { }

    public long Id { get; private set; }
    
    public int Number { get; private set; }

    public long CustomerId { get; private set; }

    public Customer Customer { get; private set; } = null!;

    public OrderStatus Status { get; set; }

    public string? Note { get; set; }
    
    public DateTime? DeliveryDate { get; set; }

    public DateTime CreatedAt { get; private set; }

    private readonly List<OrderItem> _items = [];
    public IReadOnlyCollection<OrderItem> Items => _items;

    private OrderItem GetItem(long orderItemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == orderItemId);

        return item ?? throw new DomainException(OrderErrors.ItemNotFound);
    }

    public static Order Create(long customerId, int number, string? note = null, DateTime? deliveryDate = null)
    {
        return new Order
        {
            CustomerId = customerId,
            Number = number,
            Status = OrderStatus.Pending,
            Note = note,
            DeliveryDate = deliveryDate,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    public void Update(string? note, DateTime? deliveryDate, long? customerId)
    {
        if (Status != OrderStatus.Delivered)
            DeliveryDate = deliveryDate;
        
        CustomerId = customerId ?? CustomerId;
        Note = note;
    }

    public void AddItem(long productId, int quantity, decimal unitPrice, decimal unitCost)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException(OrderErrors.OrderIsNotOpen);
        
        _items.Add(OrderItem.Create(productId, quantity, unitPrice, unitCost));
    }
    
    public void RemoveItem(long orderItemId)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException(OrderErrors.OrderIsNotOpen);

        var item = GetItem(orderItemId);

        _items.Remove(item);
    }

    public OrderItem UpdateItem(long orderItemId, int quantity, decimal unitPrice, decimal unitCost)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException(OrderErrors.OrderIsNotOpen);
        
        var item = GetItem(orderItemId);
        
        item.Update(quantity, unitPrice, unitCost);

        return item;
    }
}
