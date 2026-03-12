using BolluFlow.Application.Abstractions;

namespace BolluFlow.Application.Features.Dashboard.GetLowStockProducts;

public record GetLowStockProductsQuery : IQuery<ResultOf<List<GetLowStockProductsResult>>>;
