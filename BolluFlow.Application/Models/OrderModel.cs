using BolluFlow.Domain.Entities;
using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Models;

public class OrderModel
{
    public long Id { get; set; }

    public int Number { get; set; }

    public CustomerModel? Customer { get; set; }

    public OrderStatus Status { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? DeliveryDate { get; set; }

    public PaymentMethod? PaymentMethod { get; set; }

    public decimal Subtotal { get; set; }

    public decimal? Discount { get; set; }

    public DiscountType? DiscountType { get; set; }

    public decimal DiscountValue { get; set; }

    public decimal ShippingFee { get; set; }

    public decimal Total { get; set; }

    public OrderType Type { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public static OrderModel From(Order order)
    {
        var subtotal = order.CalculateSubtotal();
        var discountValue = order.CalculateDiscount(subtotal);

        return new OrderModel
        {
            Id = order.Id,
            Number = order.Number,
            Customer = order.Customer is null ? null : CustomerModel.From(order.Customer),
            Status = order.Status,
            Note = order.Note,
            CreatedAt = order.CreatedAt,
            DeliveryDate = order.DeliveryDate,
            PaymentMethod = order.PaymentMethod,
            Subtotal = subtotal,
            Discount = order.Discount,
            DiscountType = order.DiscountType,
            DiscountValue = discountValue,
            ShippingFee = order.ShippingFee,
            Total = subtotal - discountValue + order.ShippingFee,
            Type = order.Type,
            DeliveredAt = order.DeliveredAt
        };
    }
}
