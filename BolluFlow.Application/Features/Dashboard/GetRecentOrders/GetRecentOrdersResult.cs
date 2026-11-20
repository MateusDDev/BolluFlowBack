namespace BolluFlow.Application.Features.Dashboard.GetRecentOrders;

public sealed record GetRecentOrdersResult(
    long Id,
    int Number,
    string? CustomerName,
    string Status,
    DateTime CreatedAt,
    int TotalItems,
    decimal TotalValue);
