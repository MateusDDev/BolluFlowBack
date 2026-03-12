namespace BolluFlow.Application.Features.Dashboard.GetLowStockSupplies;

public record GetLowStockSuppliesResult(long SupplyId, string Name, decimal StockQuantity, decimal MinimumStockQuantity);
