using BolluFlow.Domain.Enums;

namespace BolluFlow.Application.Features.Reports.OrderReport;

public sealed class OrderReportResult
{
    public long Id { get; init; }
    public int Number { get; init; }
    public string? CustomerName { get; init; }
    public string? CustomerWhatsapp { get; init; }
    public string? CustomerStreet { get; init; }
    public int? CustomerNumber { get; init; }
    public string? CustomerNeighborhood { get; init; }
    public string? CustomerComplement { get; init; }
    public bool IsPickup { get; init; }
    public DateTime? DeliveryDate { get; init; }
    public decimal TotalValue { get; init; }
    public PaymentMethod? PaymentMethod { get; init; }
    public string? Note { get; init; }
    public IReadOnlyCollection<OrderReportResultItem> Items { get; init; } = [];
}

public sealed class OrderReportResultItem
{
    public string ProductName { get; init; } = null!;
    public int Quantity { get; init; }
}
