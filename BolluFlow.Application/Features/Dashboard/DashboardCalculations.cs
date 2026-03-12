namespace BolluFlow.Application.Features.Dashboard;

public static class DashboardCalculations
{
    public static string GetSeverity(decimal stockQuantity, decimal minimumStockQuantity)
    {
        return stockQuantity <= minimumStockQuantity ? "Critical" : "Warning";
    }

    public static decimal GetCoveragePercent(decimal stockQuantity, decimal minimumStockQuantity)
    {
        if (minimumStockQuantity <= 0)
            return 0;

        return decimal.Round((stockQuantity / minimumStockQuantity) * 100m, 2);
    }
}
