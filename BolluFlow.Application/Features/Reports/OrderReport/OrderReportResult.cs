namespace BolluFlow.Application.Features.Reports.OrderReport;

public sealed class OrderReportResult
{
    public long Id { get; init; }
    public int Number { get; init; }
    public string CustomerName { get; init; } = null!;
    public DateTime? DeliveryDate { get; init; }
    public string? Note { get; init; }
    public IReadOnlyCollection<OrderReportResultItem> Items { get; init; } = [];
}

public sealed class OrderReportResultItem
{
    public string ProductName { get; init; } = null!;
    public int Quantity { get; init; }
}