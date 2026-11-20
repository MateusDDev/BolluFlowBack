using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Features.Reports.CustomerOrderReport;

public sealed class CustomerOrderReportResult
{
    public long Id { get; init; }
    public int Number { get; init; }
    public string StoreName { get; init; } = null!;
    public string? CustomerName { get; init; }
    public DateTime? DeliveryDate { get; init; }
    public decimal Subtotal { get; init; }
    public decimal? Discount { get; init; }
    public DiscountType? DiscountType { get; init; }
    public decimal DiscountValue { get; init; }
    public decimal ShippingFee { get; init; }
    public decimal TotalValue { get; init; }
    public PaymentMethod? PaymentMethod { get; init; }
    public string? Note { get; init; }
    public IReadOnlyCollection<CustomerOrderReportResultItem> Items { get; init; } = [];
}

public sealed class CustomerOrderReportResultItem
{
    public string ProductName { get; init; } = null!;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}
