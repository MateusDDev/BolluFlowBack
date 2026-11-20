using BolluFlow.Domain.Abstractions;
using BolluFlow.Domain.Enums;
using BolluFlow.Domain.Errors;

namespace BolluFlow.Domain.Entities;

public class Order
{
    private Order() { }

    public long Id { get; private set; }
    
    public int Number { get; private set; }

    public long UserId { get; private set; }

    public User User { get; private set; } = null!;

    public long? CustomerId { get; private set; }

    public Customer? Customer { get; private set; }

    public OrderType Type { get; private set; }

    public bool IsDirectSale => Type == OrderType.DirectSale;

    public DateTime? DeliveredAt { get; private set; }

    public OrderStatus Status { get; private set; }

    public string? Note { get; set; }
    
    public DateTime? DeliveryDate { get; set; }
    
    public PaymentMethod? PaymentMethod { get; set; }

    public decimal? Discount { get; private set; }

    public DiscountType? DiscountType { get; private set; }

    public decimal ShippingFee { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private readonly List<OrderItem> _items = [];
    public IReadOnlyCollection<OrderItem> Items => _items;

    private OrderItem GetItem(long orderItemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == orderItemId);

        return item ?? throw new DomainException(OrderErrors.ItemNotFound);
    }

    public static Order Create(
        long userId,
        long? customerId,
        int number,
        string? note = null,
        DateTime? deliveryDate = null,
        PaymentMethod? paymentMethod = null,
        decimal? discount = null,
        DiscountType? discountType = null,
        decimal shippingFee = 0)
    {
        ValidateFinancials(discount, discountType, shippingFee, 0);

        return new Order
        {
            UserId = userId,
            CustomerId = customerId,
            Number = number,
            Type = OrderType.CustomOrder,
            Status = OrderStatus.Pending,
            Note = note,
            DeliveryDate = deliveryDate,
            PaymentMethod = paymentMethod,
            Discount = discount,
            DiscountType = discountType,
            ShippingFee = shippingFee,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    public void Update(
        string? note,
        DateTime? deliveryDate,
        long? customerId,
        PaymentMethod? paymentMethod,
        decimal? discount,
        DiscountType? discountType,
        decimal shippingFee)
    {
        ValidateFinancials(discount, discountType, shippingFee, CalculateSubtotal());

        if (Status != OrderStatus.Delivered)
            DeliveryDate = deliveryDate;
        
        CustomerId = customerId ?? CustomerId;
        Note = note;
        PaymentMethod = paymentMethod;
        Discount = discount;
        DiscountType = discountType;
        ShippingFee = shippingFee;
    }

    public void StartProduction()
    {
        EnsureCustomOrder();

        if (Status != OrderStatus.Pending)
            throw new DomainException(OrderErrors.InvalidStartTransition);

        if (_items.Count == 0)
            throw new DomainException(OrderErrors.OrderWithoutItems);

        Status = OrderStatus.InProduction;
    }

    public void FinishProduction()
    {
        EnsureCustomOrder();

        if (Status != OrderStatus.InProduction)
            throw new DomainException(OrderErrors.InvalidFinishTransition);

        Status = OrderStatus.Ready;
    }

    public void Deliver()
    {
        EnsureCustomOrder();

        if (Status != OrderStatus.Ready)
            throw new DomainException(OrderErrors.InvalidDeliverTransition);

        Status = OrderStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
    }

    public bool CanRevertStockOnCancel => Status is OrderStatus.InProduction or OrderStatus.Ready;

    public void Cancel()
    {
        EnsureCustomOrder();

        if (Status is OrderStatus.Canceled or OrderStatus.Delivered)
            throw new DomainException(OrderErrors.InvalidCancelTransition);

        Status = OrderStatus.Canceled;
    }

    public void Reopen()
    {
        EnsureCustomOrder();

        if (Status != OrderStatus.Canceled)
            throw new DomainException(OrderErrors.InvalidReopenTransition);

        Status = OrderStatus.Pending;
    }

    public void CompleteAsDirectSale()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException(OrderErrors.OrderIsNotOpen);

        if (_items.Count == 0)
            throw new DomainException(OrderErrors.DirectSaleWithoutItems);

        Type = OrderType.DirectSale;
        Status = OrderStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
    }

    public void EnsureCanBeDeleted()
    {
        if (!IsDirectSale)
            throw new DomainException(OrderErrors.OnlyDirectSaleCanBeDeleted);
    }

    private void EnsureCustomOrder()
    {
        if (IsDirectSale)
            throw new DomainException(OrderErrors.DirectSaleHasNoWorkflow);
    }

    public decimal CalculateSubtotal() =>
        _items.Sum(i => i.Quantity * i.UnitPrice);

    public decimal CalculateDiscount(decimal subtotal)
    {
        if (Discount is null || DiscountType is null)
            return 0;

        return DiscountType == Enums.DiscountType.Percentage
            ? subtotal * (Discount.Value / 100)
            : Discount.Value;
    }

    public decimal CalculateTotal()
    {
        var subtotal = CalculateSubtotal();
        var discount = CalculateDiscount(subtotal);
        return subtotal - discount + ShippingFee;
    }

    private static void ValidateFinancials(decimal? discount, DiscountType? discountType, decimal shippingFee, decimal subtotal)
    {
        if (discount is < 0)
            throw new DomainException(OrderErrors.InvalidDiscount);

        if (shippingFee < 0)
            throw new DomainException(OrderErrors.InvalidShippingFee);

        if (discount.HasValue && discountType is null)
            throw new DomainException(OrderErrors.DiscountTypeRequired);

        if (discountType == Enums.DiscountType.Percentage && discount is not null && (discount < 0 || discount > 100))
            throw new DomainException(OrderErrors.InvalidPercentageDiscount);

        if (discount is not null && discountType == Enums.DiscountType.Fixed && subtotal - discount.Value + shippingFee < 0)
            throw new DomainException(OrderErrors.NegativeTotalNotAllowed);

        if (discount is not null && discountType == Enums.DiscountType.Percentage && subtotal - (subtotal * (discount.Value / 100)) + shippingFee < 0)
            throw new DomainException(OrderErrors.NegativeTotalNotAllowed);
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
