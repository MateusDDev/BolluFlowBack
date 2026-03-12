namespace BolluFlow.Application.Features.Dashboard.GetLowStockProducts;

public record GetLowStockProductsResult(long ProductId, string Name, decimal StockQuantity, decimal MinimumStockQuantity);
