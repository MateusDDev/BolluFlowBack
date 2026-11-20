namespace BolluFlow.Application.Features.Dashboard.GetSalesSummary;

public sealed record GetSalesSummaryResult(
    SalesPeriodMetrics Current,
    SalesPeriodMetrics Previous,
    List<MonthlySales> Monthly,
    List<TopProduct> TopProducts,
    bool HasCostData);

public sealed record SalesPeriodMetrics(
    decimal Revenue,
    int SalesCount,
    decimal AverageTicket,
    decimal GrossProfit,
    decimal? Margin);

public sealed record MonthlySales(int Year, int Month, decimal Revenue, int SalesCount);

public sealed record TopProduct(long ProductId, string Name, int Quantity, decimal Revenue);
