namespace BolluFlow.Application.Features.Dashboard.GetToday;

public sealed record GetTodayResult(
    List<TodayOrder> Overdue,
    List<TodayOrder> DueToday,
    List<TodayOrder> DueTomorrow,
    List<TodayOrder> Ready);

public sealed record TodayOrder(
    long Id,
    int Number,
    string? CustomerName,
    string Status,
    DateTime? DeliveryDate,
    decimal Total);
