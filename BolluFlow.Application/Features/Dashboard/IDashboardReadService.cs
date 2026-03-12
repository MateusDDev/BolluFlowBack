namespace BolluFlow.Application.Features.Dashboard;

public interface IDashboardReadService
{
    Task<IReadOnlyList<StockAlertResponse>> GetStockAlertsAsync(int limit, CancellationToken ct);
    Task<IReadOnlyList<OrdersByStatusResponse>> GetOrdersByStatusAsync(CancellationToken ct);
    Task<IReadOnlyList<InventoryValueByTypeResponse>> GetInventoryValueByTypeAsync(CancellationToken ct);
    Task<IReadOnlyList<RecentOrderResponse>> GetRecentOrdersAsync(int limit, CancellationToken ct);
}
